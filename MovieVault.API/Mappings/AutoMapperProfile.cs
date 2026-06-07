using AutoMapper;
using MovieVault.API.Models.Domain;
using MovieVault.API.Models.DTOs;

namespace MovieVault.API.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<User, AuthResponseDto>()
            .ForMember(dest => dest.Token, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src));

        CreateMap<Watchlist, WatchlistResponseDto>();
        CreateMap<WatchlistItem, WatchlistItemResponseDto>();
        CreateMap<Review, ReviewResponseDto>();
        CreateMap<Rating, RatingResponseDto>();
        CreateMap<Favorite, FavoriteResponseDto>();
    }
}
