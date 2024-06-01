using Interns.Auth.Extensions;
using Interns.Chats.App.Dto;
using Interns.Chats.Domain;
using Interns.Chats.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Interns.Chats.App.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly ChatDbContext _dbContext;

        public ChatsController(ChatDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("direct")]
        public Task<ChatDto> CreateDirect([FromBody] CreateDirectChatDto dto)
            => Create(new CreateChatDto { Name = dto.Name, Users = [dto.UserId] });

        [HttpPost]
        public async Task<ChatDto> Create([FromBody] CreateChatDto dto)
        {
            var chat = new Chat
            {
                Name = dto.Name,
                UserIds = dto.Users,
                OwnerId = User.GetId()
            };
            _dbContext.Groups.Add(chat);

            await _dbContext.SaveChangesAsync();

            return new ChatDto
            {
                Id = chat.Id,
                Name = chat.Name
            };
        }

        [HttpGet("my")]
        public Task<List<ChatDto>> GetMyGroups()
            => _dbContext.Groups
                    .Where(x => x.UserIds.Contains(User.GetId()))
                    .Select(x => new ChatDto { Name = x.Name, Id = x.Id })
                    .ToListAsync();

        [HttpPost("{groupId}/add/{userId}")]
        public async Task AddToGroup([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var group = await _dbContext.Groups.FirstAsync(x => x.Id == groupId && x.OwnerId == User.GetId());

            if (group.UserIds.Contains(userId))
            {
                throw new BadHttpRequestException("User already in chat");
            }

            group.UserIds.Add(userId);
            await _dbContext.SaveChangesAsync();
        }

        [HttpPost("{groupId}/remove/{userId}")]
        public async Task RemoveFromGroup([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var group = await _dbContext.Groups.FirstAsync(x => x.Id == groupId && x.OwnerId == User.GetId());
            group.UserIds.Remove(userId);
            await _dbContext.SaveChangesAsync();
        }

        [HttpGet("{groupId}/messages")]
        public async Task<List<MessageDto>> GetGroupMessages([FromRoute] Guid groupId, [FromQuery] DateTime? from, [FromQuery] DateTime? until)
        {
            until ??= DateTime.UtcNow;
            from ??= until - TimeSpan.FromHours(1);

            Guid currentUserId = User.GetId();
            var messages = await _dbContext.Groups
                .Where(Chat.CanBeAccessed(groupId, currentUserId))
                .SelectMany(x => x.Messages)
                .Where(x => x.SentAt >= from && x.SentAt <= until)
                .Select(x => new MessageDto
                {
                    Id = x.Id,
                    ChatId = groupId,
                    Author = x.AuthorId,
                    Message = x.Content,
                    SentAt = x.SentAt
                })
                .ToListAsync();

            return messages;
        }
    }
}