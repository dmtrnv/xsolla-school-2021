using System.Collections.Generic;
using ProductApi.Models;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public static class SubtypeHelper
    {
        public static ProductSubtype GetOne()
        {
            return new ProductSubtype()
            {
                Name = "Merch_Leggings"
            };
        }

        public static IEnumerable<ProductSubtype> GetMany()
        {
            return new List<ProductSubtype>(5)
            {
                new ProductSubtype()
                {
                    Name = "Merch_TShirt"
                },
                new ProductSubtype()
                {
                    Name = "Game_Action"
                },
                new ProductSubtype()
                {
                    Name = "Game_MindBreaker"
                },
                new ProductSubtype()
                {
                    Name = "Merch_Headdress"
                },
                new ProductSubtype()
                {
                    Name = "Game_Rpg"
                }
            };
        }
    }
}
