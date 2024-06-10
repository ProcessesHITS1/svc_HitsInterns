using AutoMapper;
using Microsoft.EntityFrameworkCore;
using svc_InterviewBack.DAL;
using svc_InterviewBack.Models;
using svc_InterviewBack.Services.Clients;
using svc_InterviewBack.Utils;

namespace svc_InterviewBack.Services;

using static svc_InterviewBack.Services.Clients.ThirdCourseClient;
using Season = Models.Season;
using SeasonDb = DAL.Season;

public interface ISeasonsService
{
    // methods for clients
    // Here input and output models are both DTOs
    public Task<List<Season>> GetAll();
    public Task<Season> Get(int year);
    public Task<Season> Create(SeasonData seasonData);
    public Task<Season> Update(int year, SeasonData seasonData);
    public Task Delete(int year);
    public Task Close(int year);

    // extra methods for internal use
    public Task<SeasonDb> Find(int year, bool withCompanies = true, bool withStudents = true, bool checkOpen = false);
}

public class SeasonsService(InterviewDbContext context, IMapper mapper, ThirdCourseClient client) : ISeasonsService
{
    public async Task Close(int year)
    {
        var season = await context.Seasons.Include(s => s.Students).ThenInclude(s => s.Company).FirstOrDefaultAsync(s => s.Year == year)
                     ?? throw new NotFoundException($"Season with year {year} not found");
        if (season.IsClosed) throw new BadRequestException($"Season with year {year} is already closed");
        season.IsClosed = true;

        var request = season.Students.Where(s => s.EmploymentStatus == EmploymentStatus.Employed).Select(s => new StudentInfo
        {
            StudentId = s.Id,
            CompanyId = s.Company?.Id ?? throw new BadRequestException($"Student with id {s.Id} is employed but has no company"),
        }).ToList();
        await Task.WhenAll(
            client.AddStudentsToSemester(new StudentsInternship { Students = request, Year = year }),
            context.SaveChangesAsync());
    }

    public async Task<Season> Get(int year)
    {
        var season = await context.Seasons.FirstOrDefaultAsync(s => s.Year == year)
                     ?? throw new NotFoundException($"Season with year {year} not found");
        return mapper.Map<Season>(season);
    }

    public async Task<Season> Create(SeasonData seasonData)
    {
        if (seasonData.SeasonStart >= seasonData.SeasonEnd)
            throw new BadRequestException("Season start date should be before season end date");
        if (await context.Seasons.AnyAsync(s => s.Year == seasonData.Year))
            throw new BadRequestException($"Season with year {seasonData.Year} already exists");
        var season = mapper.Map<SeasonDb>(seasonData);
        context.Add(season);
        await context.SaveChangesAsync();
        return mapper.Map<Season>(season);
    }

    public async Task Delete(int year)
    {
        var season = await context.Seasons.FirstOrDefaultAsync(s => s.Year == year)
                     ?? throw new NotFoundException($"Season with year {year} not found");
        context.Remove(season);
        await context.SaveChangesAsync();
    }

    public async Task<SeasonDb> Find(int year, bool withCompanies = true, bool withStudents = true, bool checkOpen = false)
    {
        var query = context.Seasons.AsQueryable();
        if (withCompanies)
            query = query.Include(s => s.Companies);
        if (withStudents)
            query = query.Include(s => s.Students);

        var season = await query.FirstOrDefaultAsync(s => s.Year == year)
                     ?? throw new NotFoundException($"Season with year {year} not found");
        if (checkOpen && season.IsClosed)
            throw new BadRequestException($"Season with year {year} is closed");
        return season;
    }

    public async Task<List<Season>> GetAll()
    {
        var seasons = await context.Seasons.ToListAsync();
        return mapper.Map<List<Season>>(seasons);
    }

    public async Task<Season> Update(int year, SeasonData seasonData)
    {
        var season = await context.Seasons.FirstOrDefaultAsync(s => s.Year == year)
                     ?? throw new NotFoundException($"Season with year {year} not found");
        mapper.Map(seasonData, season);
        await context.SaveChangesAsync();
        return mapper.Map<Season>(season);
    }
}