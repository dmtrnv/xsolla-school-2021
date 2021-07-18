using AutoMapper;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Profiles
{
    public class ProductTypeProfile : Profile
    {
        public ProductTypeProfile()
        {
            CreateMap<ProductType, ProductTypeCreateUpdateDto>();

            CreateMap<ProductTypeCreateUpdateDto, ProductType>();

            CreateMap<ProductType, ProductTypeReadDto>();

            CreateMap<ProductTypeReadDto, ProductType>();
        }
    }
}
