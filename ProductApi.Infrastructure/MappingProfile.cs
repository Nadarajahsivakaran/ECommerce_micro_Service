using AutoMapper;
using ProductApi.Models;
using ProductApi.Models.DTO;

namespace ProductApi.Infrastructure
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
			CreateMap<CategoryCreateDto, Category>().ReverseMap();
			CreateMap<CategoryDto, Category>().ReverseMap();
		}
        
    }
}
