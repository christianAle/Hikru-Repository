using AutoMapper;

namespace HikruCodeChallenge.Application.Mappings
{
    public static class MapperBuilder
    {
        public static IMapper Create()
        {
            return new AutoMapper.MapperConfiguration(options =>
            {
                options.AddProfile(new AssessmentProfile());
            }).CreateMapper();
        }
    }
    
}
