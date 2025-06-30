using AutoMapper;
using HikruCodeChallenge.Domain.Entities;
using HikruCodeChallenge.Application.DTOs;

namespace HikruCodeChallenge.Application.Mappings;

public class AssessmentProfile : Profile
{
    public AssessmentProfile()
    {
        CreateMap<Assessment, AssessmentDto>();

        CreateMap<UpdateAssessmentDto, Assessment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());

        CreateMap<CreateAssessmentDto, Assessment>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore());
    }
}
