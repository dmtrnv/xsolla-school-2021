using System.Collections.Generic;
using ProductApi.Models;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public static class TypeHelper
    {
        public static ProductType GetOne()
        {
            return new ProductType()
            {
                Name = "VirtualCurrency"
            };
        }

        public static IEnumerable<ProductType> GetMany()
        {
            return new List<ProductType>(2)
            {
                new ProductType()
                {
                    Name = "Game"
                },
                new ProductType()
                {
                    Name = "Merch"
                }
            };
        }
    }
}
