using AutoMapper;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductCreateUpdateDto>();

            CreateMap<ProductCreateUpdateDto, Product>();

            CreateMap<Product, ProductReadDto>();

            CreateMap<ProductReadDto, Product>();
        }
    }
}
