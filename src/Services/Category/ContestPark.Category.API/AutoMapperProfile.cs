using AutoMapper;
using ContestPark.Category.API.Infrastructure.Documents;
using ContestPark.Category.API.IntegrationEvents.Events;
using ContestPark.Category.API.Model;

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