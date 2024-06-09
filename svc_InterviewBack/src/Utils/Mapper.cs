using AutoMapper;
using svc_InterviewBack.Models;
using svc_InterviewBack.DAL;

namespace svc_InterviewBack.Utils;

using SeasonDb = DAL.Season;
using Season = Models.Season;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<SeasonDb, Season>();
        CreateMap<SeasonData, SeasonDb>();
        CreateMap<SeasonDb, SeasonDetails>()
                .ForMember(dest => dest.Season, opt => opt.MapFrom(src => new Season
                (src.Id, src.Year, src.SeasonStart, src.SeasonEnd)));

        CreateMap<Company, CompanyInSeasonInfo>()
            .ForMember(dest => dest.NPositions, opt => opt.MapFrom(src => src.Positions.Count))
            .ForMember(dest => dest.SeasonYear, opt => opt.MapFrom(src => src.Season.Year));

        CreateMap<Position, PositionInfo>();

        CreateMap<Student, StudentInfo>()
            .ForMember(dest => dest.EmploymentStatus, opt => opt.MapFrom(src => src.EmploymentStatus.ToString()));

        CreateMap<Position, PositionInfo>();

        CreateMap<CompanyAndPosition, PositionInfo>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Position.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Position.Title))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Position.Description))
            .ForMember(dest => dest.NSeats, opt => opt.MapFrom(src => src.Position.NSeats))
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Company.Id))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.Name))
            .ForMember(dest => dest.SeasonYear, opt => opt.MapFrom(src => src.Company.Season.Year));

        CreateMap<(Position, Student), InterviewRequest>()
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Item1))
            .ForMember(dest => dest.Student, opt => opt.MapFrom(src => src.Item2));

        CreateMap<InterviewRequest, RequestDetails>()
            .ForMember(dest => dest.PositionId, opt => opt.MapFrom(src => src.Position.Id))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.Student.Id));

        CreateMap<PositionData, Position>();

    }

}