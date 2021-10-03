using System.Collections.Generic;
using ProductApi.Models;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public static class ManufacturerHelper
    {
        public static ProductManufacturer GetOne()
        {
            return new ProductManufacturer
            {
                Name = "XSolla",
                Abbreviation = "XSLL"
            };
        }

        public static IEnumerable<ProductManufacturer> GetMany()
        {
            return new List<ProductManufacturer>(3)
            {
                new ProductManufacturer()
                {
                    Name = "Valve",
                    Abbreviation = "VLV"
                },
                new ProductManufacturer()
                {
                    Name = "Epic Games",
                    Abbreviation = "PCGMS"
                },
                new ProductManufacturer()
                {
                    Name = "Paradise",
                    Abbreviation = "PRDS"
                }
            };
        }
    }
}
