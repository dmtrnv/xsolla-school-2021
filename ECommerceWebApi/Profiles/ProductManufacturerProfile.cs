using AutoMapper;
using ProductApi.DataTransferObjects;
using ProductApi.Models;

namespace ProductApi.Profiles
{
    public class ProductManufacturerProfile : Profile
    {
        public ProductManufacturerProfile()
        {
            CreateMap<ProductManufacturer, ProductManufacturerCreateUpdateDto>();

            CreateMap<ProductManufacturerCreateUpdateDto, ProductManufacturer>();

            CreateMap<ProductManufacturer, ProductManufacturerReadDto>();

            CreateMap<ProductManufacturerReadDto, ProductManufacturer>();
        }
    }
}
