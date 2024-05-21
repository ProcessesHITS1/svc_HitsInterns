namespace svc_InterviewBack.Models;

// Basic season models
// ################

// Creation model
public record SeasonData(int Year, DateTime SeasonStart, DateTime SeasonEnd);

// Returned to the client
public record Season(Guid Id, int Year, DateTime SeasonStart, DateTime SeasonEnd);

// Returned to the client
public record SeasonDetails
{
    public required Season Season { get; init; }
    public required List<CompanyInSeasonInfo> Companies { get; init; }
    public required List<StudentInfo> Students { get; init; }
};

// Company related models

// Returned to the client
public record CompanyInSeasonInfo
{
    public Guid Id { get; init; }
    public int SeasonYear { get; init; }
    public required string Name { get; init; }
    public required int NPositions { get; init; }
}


// Returned to the client
public record StudentInfo
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public required string EmploymentStatus { get; init; }
}