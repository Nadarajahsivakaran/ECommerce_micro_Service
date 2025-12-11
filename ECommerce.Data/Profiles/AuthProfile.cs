using AutoMapper;
using AuthApi.Models;
using AuthApi.Models.DTO;

namespace ECommerce.Data.Profiles
{
	public class AuthProfile : Profile
	{
		public AuthProfile()
		{
			CreateMap<RegisterDto, ApplicationUser>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
				.ReverseMap();
			CreateMap<ApplicationUser, RegisterResponseDto>()
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
				.ReverseMap();

		}
	}
}
