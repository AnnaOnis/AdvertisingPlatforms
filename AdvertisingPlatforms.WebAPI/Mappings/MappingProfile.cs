using AdvertisingPlatforms.Domain.Models;
using AdvertisingPlatforms.Web.HttpModels.Requests;
using AdvertisingPlatforms.Web.HttpModels.Responses;
using AutoMapper;

namespace AdvertisingPlatforms.Web.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Advertisement, AdvertisementResponse>();
            CreateMap<AdvertisingPlatform, AdvertisingPlatformResponse>();
            CreateMap<Location, LocationResponse>();
        }
    }
}
