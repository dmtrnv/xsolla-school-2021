using System;

namespace ProductApi.Models
{
    public class Product
    {
        public Guid Id { get; set; }

        public string Sku { get; set; }

        public string Name { get; set; }

        public Guid ManufacturerId { get; set; }
        public ProductManufacturer Manufacturer { get; set; }

        public Guid TypeId { get; set; }
        public ProductType Type { get; set; }

        public Guid SubtypeId { get; set; }
        public ProductSubtype Subtype { get; set; }

        public decimal Cost { get; set; }
    }
}
