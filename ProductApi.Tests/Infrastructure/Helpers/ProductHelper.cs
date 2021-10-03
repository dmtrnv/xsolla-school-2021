using System.Collections.Generic;
using ProductApi.Models;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public static class ProductHelper
    {
        public static Product GetOne()
        {
            return new Product()
            {
                Name = "Counter Strike:Global Offensive",
                Manufacturer = new ProductManufacturer()
                {
                    Name = "Valve",
                    Abbreviation = "VLV"
                },
                Type = new ProductType()
                {
                    Name = "Game"
                },
                Subtype = new ProductSubtype()
                {
                    Name = "Game_Action"
                },
                Cost = 29.99M,
                Count = 122
            };
        }

        public static IEnumerable<Product> GetMany()
        {
            return new List<Product>(3)
            {
                new Product()
                {
                    Name = "Dota 2",
                    Manufacturer = new ProductManufacturer()
                    {
                        Name = "Valve",
                        Abbreviation = "VLV"
                    },
                    Type = new ProductType()
                    {
                        Name = "Game"
                    },
                    Subtype = new ProductSubtype()
                    {
                        Name = "Game_Rpg"
                    },
                    Cost = 15.99M,
                    Count = 255
                },
                new Product()
                {
                    Name = "Misha power",
                    Manufacturer = new ProductManufacturer()
                    {
                        Name = "Paradise",
                        Abbreviation = "PRDS"
                    },
                    Type = new ProductType()
                    {
                        Name = "Merch"
                    },
                    Subtype = new ProductSubtype()
                    {
                        Name = "Merch_TShirt"
                    },
                    Cost = 9.99M,
                    Count = 300
                },
                new Product()
                {
                    Name = "Fortnite",
                    Manufacturer = new ProductManufacturer()
                    {
                        Name = "Epic Games",
                        Abbreviation = "PCGMS"
                    },
                    Type = new ProductType()
                    {
                        Name = "Game"
                    },
                    Subtype = new ProductSubtype()
                    {
                        Name = "Game_Action"
                    },
                    Cost = 19.99M,
                    Count = 50
                }
            };
        }
    }
}
