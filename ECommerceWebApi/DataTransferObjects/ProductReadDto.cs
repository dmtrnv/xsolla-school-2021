using System;

namespace ProductApi.DataTransferObjects
{
    public class ProductReadDto
    {
        public Guid Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public ProductManufacturerCreateUpdateDto Manufacturer { get; set; }

        public ProductTypeCreateUpdateDto Type { get; set; }

        public ProductSubtypeCreateUpdateDto Subtype { get; set; }

        public decimal Cost { get; set; }

        public int Count { get; set; }
    }
}