using AutoMapper;
using ProductApi.Models;
using ProductApi.Models.DTO;

namespace ECommerce.Data.Profiles
{
    public class ProductProfile : Profile
	{
		public ProductProfile()
		{
			CreateMap<CategoryCreateDto, Category>().ReverseMap();
			CreateMap<CategoryDto, Category>().ReverseMap();
			CreateMap<Product,ProductDto>();
		}
	}
}
