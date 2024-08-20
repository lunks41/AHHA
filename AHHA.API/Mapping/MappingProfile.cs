using AHHA.Core.Entities.Masters;
using AHHA.Core.Models.Masters;
using AutoMapper;

namespace AHHA.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CountryLookupViewModel, M_Country>().ReverseMap();
            CreateMap<ProductViewModel, M_Product>().ReverseMap();
        }
    }
}