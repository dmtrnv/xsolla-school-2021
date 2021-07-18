using System;

namespace ProductApi.DataTransferObjects
{
    public class ProductTypeReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }
    }
}
