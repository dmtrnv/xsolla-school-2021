using AutoMapper;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Profiles
{
    public class ProductSubtypeProfile : Profile
    {
        public ProductSubtypeProfile()
        {
            CreateMap<ProductSubtype, ProductSubtypeCreateUpdateDto>();

            CreateMap<ProductSubtypeCreateUpdateDto, ProductSubtype>();

            CreateMap<ProductSubtype, ProductSubtypeReadDto>();

            CreateMap<ProductSubtypeReadDto, ProductSubtype>();
        }
    }
}
