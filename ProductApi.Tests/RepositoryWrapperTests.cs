using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductApi.Misc;
using ProductApi.Models;
using ProductApi.Tests.Infrastructure.Fixtures;
using ProductApi.Tests.Infrastructure.Helpers;
using Xunit;

namespace ProductApi.Tests
{
    public class RepositoryWrapperTests : IClassFixture<RepositoryWrapperFixture>
    {
        private readonly RepositoryWrapperFixture _fixture;

        public RepositoryWrapperTests(RepositoryWrapperFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetAllProductsAsync_WhenAnyProductExists_ReturnsNotNull()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var products = await sut.Products.GetAllProductsAsync();

            // assert
            Assert.NotNull(products);
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetAllProductsAsync_WhenAnyProductExists_ReturnsProductWithItsManufacturerIdAndTypeIdAndSubtypeId()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var products = await sut.Products.GetAllProductsAsync();

            // assert
            foreach (var product in products)
            {
                Assert.NotEqual(Guid.Empty, product.ManufacturerId);
                Assert.NotEqual(Guid.Empty, product.TypeId);
                Assert.NotEqual(Guid.Empty, product.SubtypeId);
            }
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetAllProductsWithDetailsAsync_WhenAnyProductExists_ReturnsProductWithItsManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var products = await sut.Products.GetAllProductsWithDetailsAsync();

            // assert
            foreach (var product in products)
            {
                Assert.NotNull(product.Manufacturer);
                Assert.NotNull(product.Type);
                Assert.NotNull(product.Subtype);
            }
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetProductsFilteredByCostWithDetails_WhenProductWithCostInRangeExists_ReturnsExpectedAmountOfProducts()
        {
            // arrange
            var sut = _fixture.Create();
            var expected = 2;

            // act
            var products = await sut.Products.GetProductsFilteredByCostWithDetails(new ProductParameters
                {
                    MinCost = 8M,
                    MaxCost = 17M
                })
                .ToListAsync();

            // assert
            Assert.Equal(expected, products.Count);
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetProductsFilteredByCostWithDetails_WhenProductWithCostInRangeExists_ReturnsProductWithItsManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var products = await sut.Products.GetProductsFilteredByCostWithDetails(new ProductParameters
                {
                    MinCost = 8M,
                    MaxCost = 17M
                })
                .ToListAsync();

            // assert
            foreach (var product in products)
            {
                Assert.NotNull(product.Manufacturer);
                Assert.NotNull(product.Type);
                Assert.NotNull(product.Subtype);
            }
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public void GetProductsFilteredByCostWithDetails_WhenProductParametersIsNull_ThrowsNullReferenceException()
        {
            // arrange
            var sut = _fixture.Create();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => sut.Products.GetProductsFilteredByCostWithDetails(null));
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetProductsFilteredByCostAndTypeNameWithDetails_WhenProductWithCostInRangeAndTypeNameExists_ReturnsExpectedAmountOfProducts()
        {
            // arrange
            var sut = _fixture.Create();
            var expected = 1;

            // act
            var products = await sut.Products.GetProductsFilteredByCostAndTypeNameWithDetails(new ProductParameters
                {
                    MinCost = 8M,
                    MaxCost = 17M,
                    TypeName = "Merch"
                })
                .ToListAsync();

            // assert
            Assert.Equal(expected, products.Count);
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetProductsFilteredByCostAndTypeNameWithDetails_WhenProductWithCostInRangeAndTypeNameExists_ReturnsProductWithItsManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var products = await sut.Products.GetProductsFilteredByCostAndTypeNameWithDetails(new ProductParameters
                {
                    MinCost = 8M,
                    MaxCost = 17M,
                    TypeName = "Merch"
                })
                .ToListAsync();

            // assert
            foreach (var product in products)
            {
                Assert.NotNull(product.Manufacturer);
                Assert.NotNull(product.Type);
                Assert.NotNull(product.Subtype);
            }
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public async void GetProductsFilteredByCostAndTypeNameWithDetails_WhenProductWithCostInRangeAndTypeNameExistsAndProductParametersTypeNameIsNull_ReturnsNoProducts()
        {
            // arrange
            var sut = _fixture.Create();
            var expected = 0;

            // act
            var products = await sut.Products.GetProductsFilteredByCostAndTypeNameWithDetails(new ProductParameters
                {
                    MinCost = 8M,
                    MaxCost = 17M,
                    TypeName = null
                })
                .ToListAsync();

            // assert
            Assert.Equal(expected, products.Count);
        }

        [Fact]
        [Trait("ProductRepository", "GetProducts")]
        public void GetProductsFilteredByCostAndTypeNameWithDetails_WhenProductParametersIsNull_ThrowsNullReferenceException()
        {
            // arrange
            var sut = _fixture.Create();

            // act

            // assert
            Assert.Throws<ArgumentNullException>(() => sut.Products.GetProductsFilteredByCostAndTypeNameWithDetails(null));
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductByIdAsync_WhenProductWithIdExists_ReturnsActualProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var productId = (await sut.Products.FindAll().FirstOrDefaultAsync()).Id;

            // act
            var product = await sut.Products.GetProductByIdAsync(productId);

            // assert
            Assert.Equal(productId, product.Id);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductByIdAsync_WhenProductWithIdNotExists_ReturnsNull()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var product = await sut.Products.GetProductByIdAsync(Guid.NewGuid());

            // assert
            Assert.Null(product);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductByIdWithDetailsAsync_WhenProductWithIdExists_ReturnsActualProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var productId = (await sut.Products.FindAll().FirstOrDefaultAsync()).Id;

            // act
            var product = await sut.Products.GetProductByIdWithDetailsAsync(productId);

            // assert
            Assert.Equal(productId, product.Id);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductByIdWithDetailsAsync_WhenProductWithIdExists_ReturnsProductWithItsManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();
            var productId = (await sut.Products.FindAll().FirstOrDefaultAsync()).Id;

            // act
            var product = await sut.Products.GetProductByIdWithDetailsAsync(productId);

            // assert
            Assert.NotNull(product.Manufacturer);
            Assert.NotNull(product.Type);
            Assert.NotNull(product.Subtype);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductByIdWithDetailsAsync_WhenProductWithIdNotExists_ReturnsNull()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var product = await sut.Products.GetProductByIdWithDetailsAsync(Guid.NewGuid());

            // assert
            Assert.Null(product);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductBySkuAsync_WhenProductWithSkuExists_ReturnsActualProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var productSku = (await sut.Products.FindAll().FirstOrDefaultAsync()).Sku;

            // act
            var product = await sut.Products.GetProductBySkuAsync(productSku);

            // assert
            Assert.Equal(productSku, product.Sku);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductBySkuAsync_WhenProductWithSkuNotExists_ReturnsNull()
        {
            // arrange
            var sut  = _fixture.Create();

            // act
            var product = await sut.Products.GetProductBySkuAsync("PPPLSAS-25-1-DSD");

            // assert
            Assert.Null(product);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductBySkuWithDetailsAsync_WhenProductWithSkuExists_ReturnsActualProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var productSku = (await sut.Products.FindAll().FirstOrDefaultAsync()).Sku;

            // act
            var product = await sut.Products.GetProductBySkuWithDetailsAsync(productSku);

            // assert
            Assert.Equal(productSku, product.Sku);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductBySkuWithDetailsAsync_WhenProductWithSkuExists_ReturnsProductWithItsManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();
            var productSku = (await sut.Products.FindAll().FirstOrDefaultAsync()).Sku;

            // act
            var product = await sut.Products.GetProductBySkuWithDetailsAsync(productSku);

            // assert
            Assert.NotNull(product.Manufacturer);
            Assert.NotNull(product.Type);
            Assert.NotNull(product.Subtype);
        }

        [Fact]
        [Trait("ProductRepository ", "GetProduct")]
        public async void GetProductBySkuWithDetailsAsync_WhenProductWithSkuNotExists_ReturnsNull()
        {
            // arrange
            var sut = _fixture.Create();

            // act
            var product = await sut.Products.GetProductBySkuAsync("PPPLSAS-25-1-DSD");

            // assert
            Assert.Null(product);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenManufacturerAndTypeAndSubtypeExists_CreatesProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var product = ProductHelper.GetOne();

            // act
            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var createdProductId = (await sut.Products.FindByCondition(p => p.Name == product.Name).FirstOrDefaultAsync()).Id;
            var createdProduct = await sut.Products.GetProductByIdWithDetailsAsync(createdProductId);
            
            Assert.NotNull(createdProduct);
            Assert.NotEqual(Guid.Empty, createdProduct.ManufacturerId);
            Assert.NotEqual(Guid.Empty, createdProduct.TypeId);
            Assert.NotEqual(Guid.Empty, createdProduct.SubtypeId);
            Assert.NotNull(createdProduct.Manufacturer);
            Assert.NotNull(createdProduct.Type);
            Assert.NotNull(createdProduct.Subtype);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenManufacturerNotExists_CreatesProductAndManufacturer()
        {
            // arrange
            var sut = _fixture.Create();
            var product = new Product
            {
                Name = "Solomone",
                Manufacturer = new ProductManufacturer()
                {
                    Name = "Solomon",
                    Abbreviation = "SLMN"
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
            };

            // act
            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var createdProduct = await sut.Products.FindByCondition(p => p.Name == "Solomone").FirstOrDefaultAsync();
            var createdManufacturer = await sut.Manufacturers.FindByCondition(m => (m.Name == "Solomon") && (m.Abbreviation == "SLMN")).FirstOrDefaultAsync();

            Assert.NotNull(createdProduct);
            Assert.NotNull(createdManufacturer);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenTypeNotExists_CreatesProductAndType()
        {
            // arrange
            var sut = _fixture.Create();
            var product = new Product
            {
                Name = "The MOon",
                Manufacturer = new ProductManufacturer()
                {
                    Name = "Paradise",
                    Abbreviation = "PRDS"
                },
                Type = new ProductType()
                {
                    Name = "VirtualCurrency"
                },
                Subtype = new ProductSubtype()
                {
                    Name = "VirtualCurrency_InGame"
                },
                Cost = 9.99M,
                Count = 300
            };

            // act
            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var createdProduct = await sut.Products.FindByCondition(p => p.Name == "The MOon").FirstOrDefaultAsync();
            var createdType = await sut.Types.FindByCondition(t => t.Name == "VirtualCurrency").FirstOrDefaultAsync();

            Assert.NotNull(createdProduct);
            Assert.NotNull(createdType);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenSubtypeNotExists_CreatesProductAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();
            var product = new Product
            {
                Name = "ChiChi",
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
                    Name = "Merch_Leggings"
                },
                Cost = 9.99M,
                Count = 300
            };

            // act
            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var createdProduct = await sut.Products.FindByCondition(p => p.Name == "ChiChi").FirstOrDefaultAsync();
            var createdSubtype = await sut.Subtypes.FindByCondition(s => s.Name == "Merch_Leggings").FirstOrDefaultAsync();

            Assert.NotNull(createdProduct);
            Assert.NotNull(createdSubtype);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenManufacturerAndTypeAndSubtypeNotExists_CreatesProductAndManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();
            var product = new Product
            {
                Name = "The Moon",
                Manufacturer = new ProductManufacturer()
                {
                    Name = "Epic games store",
                    Abbreviation = "PCGMSTR"
                },
                Type = new ProductType()
                {
                    Name = "VirtualCurrency"
                },
                Subtype = new ProductSubtype()
                {
                    Name = "VirtualCurrency_InGame"
                },
                Cost = 9.99M,
                Count = 300
            };

            // act
            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var createdProduct = await sut.Products.FindByCondition(p => p.Name == "The Moon").FirstOrDefaultAsync();
            var createdManufacturer = await sut.Manufacturers.FindByCondition(m => (m.Name == "Epic games store") && (m.Abbreviation == "PCGMSTR")).FirstOrDefaultAsync();
            var createdType = await sut.Types.FindByCondition(t => t.Name == "VirtualCurrency").FirstOrDefaultAsync();
            var createdSubtype = await sut.Subtypes.FindByCondition(s => s.Name == "VirtualCurrency_InGame").FirstOrDefaultAsync();
            
            Assert.NotNull(createdProduct);
            Assert.NotNull(createdManufacturer);
            Assert.NotNull(createdType);
            Assert.NotNull(createdSubtype);
        }

        [Fact]
        [Trait("ProductRepository ", "CreateProduct")]
        public async void CreateProductAsync_WhenSameProductExists_ThrowsInvalidOperationException()
        {
            // arrange
            var sut = _fixture.Create();
            var product = (await sut.Products.GetAllProductsWithDetailsAsync()).First();

            // act

            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.Products.CreateProductAsync(product));
        }

        [Fact]
        [Trait("ProductRepository ", "UpdateProduct")]
        public async void UpdateProductAsync_WhenManufacturerAndTypeAndSubtypeExists_UpdatesProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var product = ProductHelper.GetOne();

            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            var manufacturer = (await sut.Manufacturers.GetAllManufacturersAsync()).First();
            var type = (await sut.Types.GetAllTypesAsync()).First();
            var subtype = (await sut.Subtypes.GetAllSubtypesAsync()).First();

            // act
            product.Manufacturer = manufacturer;
            product.Type = type;
            product.Subtype = subtype;

            await sut.Products.UpdateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var updatedProduct = await sut.Products.GetProductByIdWithDetailsAsync(product.Id);

            Assert.Equal(updatedProduct.Manufacturer.Name, manufacturer.Name);
            Assert.Equal(updatedProduct.Manufacturer.Abbreviation, manufacturer.Abbreviation);
            Assert.Equal(updatedProduct.Type.Name, type.Name);
            Assert.Equal(updatedProduct.Subtype.Name, subtype.Name);
        }

        [Fact]
        [Trait("ProductRepository ", "UpdateProduct")]
        public async void UpdateProductAsync_WhenManufacturerAndTypeAndSubtypeNotExists_UpdatesProductAndCreatesManufacturerAndTypeAndSubtype()
        {
            // arrange
            var sut = _fixture.Create();
            var product = ProductHelper.GetOne();

            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            var manufacturer = new ProductManufacturer
            {
                Name = "Fabrica",
                Abbreviation = "FBRC"
            };
            var type = new ProductType { Name = "VirtualCurrency" };
            var subtype = new ProductSubtype { Name = "VirtualCurrency_InGame" };

            // act
            product.Manufacturer = manufacturer;
            product.Type = type;
            product.Subtype = subtype;

            await sut.Products.UpdateProductAsync(product);
            await sut.SaveAsync();

            // assert
            var updatedProduct = await sut.Products.GetProductByIdWithDetailsAsync(product.Id);

            Assert.Equal(updatedProduct.Manufacturer.Name, manufacturer.Name);
            Assert.Equal(updatedProduct.Manufacturer.Abbreviation, manufacturer.Abbreviation);
            Assert.Equal(updatedProduct.Type.Name, type.Name);
            Assert.Equal(updatedProduct.Subtype.Name, subtype.Name);
            Assert.NotNull(await sut.Manufacturers.GetManufacturerByIdAsync(updatedProduct.ManufacturerId));
            Assert.NotNull(await sut.Types.GetTypeByIdAsync(updatedProduct.TypeId));
            Assert.NotNull(await sut.Subtypes.GetSubtypeById(updatedProduct.SubtypeId));
        }

        [Fact]
        [Trait("ProductRepository", "DeleteProduct")]
        public async void DeleteProduct_WhenProductExists_DeletesProduct()
        {
            // arrange
            var sut = _fixture.Create();
            var product = ProductHelper.GetOne();

            await sut.Products.CreateProductAsync(product);
            await sut.SaveAsync();

            // act
            sut.Products.DeleteProduct(product);
            await sut.SaveAsync();

            // assert
            Assert.Null(await sut.Products.FindByCondition(p => p.Name == ProductHelper.GetOne().Name).FirstOrDefaultAsync());
        }
    }
}
