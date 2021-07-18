using System;

namespace ProductApi.DataTransferObjects
{
    public class ProductManufacturerReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Abbreviation { get; set; }
    }
}