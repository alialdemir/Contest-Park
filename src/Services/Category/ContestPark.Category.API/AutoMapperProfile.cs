using AutoMapper;
using ContestPark.Category.API.Infrastructure.Tables;
using ContestPark.Category.API.Models;

namespace ContestPark.Category.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Search, SearchModel>();
        }
    }
}
