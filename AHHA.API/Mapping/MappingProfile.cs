using AutoMapper;
using AHHA.Core.Entities.Admin;
using AHHA.Core.Entities.Masters;
using AHHA.Core.Models;


namespace AHHA.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CountryViewModel, M_Country>().ReverseMap();
            CreateMap<ProductViewModel, M_Product>().ReverseMap();
        }
    }
}
