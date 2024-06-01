﻿namespace Interns.Chats.App.Dto
{
    public class CreateDirectChatDto
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
    public class CreateChatDto
    {
        public string Name { get; set; }
        public List<Guid> Users { get; set; }
    }
}