using AutoMapper;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductCreateDto>();

            CreateMap<ProductCreateDto, Product>();

            CreateMap<Product, ProductReadDto>();

            CreateMap<ProductReadDto, Product>();

            CreateMap<Product, ProductUpdateDto>();

            CreateMap<ProductUpdateDto, Product>();
        }
    }
}
