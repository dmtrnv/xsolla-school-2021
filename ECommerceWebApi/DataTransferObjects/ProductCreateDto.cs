namespace ProductApi.DataTransferObjects
{
    public class ProductCreateDto
    {
        public string Name { get; set; }

        public ProductManufacturerCreateUpdateDto Manufacturer { get; set; }

        public ProductTypeCreateUpdateDto Type { get; set; }

        public ProductSubtypeCreateUpdateDto Subtype { get; set; }

        public decimal Cost { get; set; }
    }
}
