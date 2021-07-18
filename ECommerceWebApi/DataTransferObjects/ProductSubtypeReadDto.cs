using System;

namespace ProductApi.DataTransferObjects
{
    public class ProductSubtypeReadDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }
    }
}