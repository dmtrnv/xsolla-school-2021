using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ProductApi.Models;
using ProductApi.Tests.Infrastructure.Helpers;
using Xunit;

namespace ProductApi.IntegrationTests
{
    public class ProductsControllerTests : IntegrationTest
    {
        [Fact]
        [Trait("GetAllProductsAsync", "")]
        public async Task GetAllProductsAsync_WhenProductsExists_ReturnsActualProductsAndOk()
        {
            // arrange


            // act
            var response = await TestClient.GetAsync("api/products");

            var products = await response.Content.ReadFromJsonAsync<List<Product>>();

            var totalCountFromHeader = GetTotalCountOfProductsFromHeaderPagination(response);

            // assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(ProductHelper.GetMany().Count(), products.Count);
            Assert.Equal(products.Count, totalCountFromHeader);
        }

        [Fact]
        [Trait("GetAllProductsAsync", "")]
        public async Task GetAllProductsAsync_WhenProductParametersCostIsNotValid_ReturnsBadRequest()
        {
            // arrange


            // act
            var response = await TestClient.GetAsync("api/products?MinCost=20&MaxCost=10");

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [Fact]
        [Trait("GetProductByIdOrSkuAsync", "")]
        public async Task GetProductByIdOrSkuAsync_WhenProductExists_ReturnsActualProductAndOk()
        {
            // arrange
            var getProductsResponse = await TestClient.GetAsync("api/products");

            var existingProduct = (await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>()).First();

            // act
            var response = await TestClient.GetAsync($"api/products/{existingProduct.Id}");

            var product = await response.Content.ReadFromJsonAsync<Product>();

            // assert
            Assert.Equal(existingProduct.Id, product.Id);
            Assert.Equal(existingProduct.Sku, product.Sku);
            Assert.Equal(existingProduct.Name, product.Name);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        [Trait("GetProductByIdOrSkuAsync", "")]
        public async Task GetProductByIdOrSkuAsync_WhenProductNotExists_ReturnsNotFound()
        {
            // arrange


            // act
            var response = await TestClient.GetAsync($"api/products/{Guid.NewGuid()}");

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("CreateProductAsync", "")]
        public async Task CreateProductAsync_WhenProductIsValid_CreatesProductAndReturnsCreatedAtAction()
        {
            // arrange
            var product = ProductHelper.GetOne();

            // act
            var response = await TestClient.PostAsJsonAsync("api/products", product);

            var getProductsResponse = await TestClient.GetAsync("api/products");

            var products = await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>();

            // assert
            Assert.Contains(products, p => p.Name.Equals(product.Name));
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(response.Headers.GetValues("Location"));
            Assert.NotEmpty(response.Headers.GetValues("Location"));
        }

        [Fact]
        [Trait("CreateProductAsync", "")]
        public async Task CreateProductAsync_WhenProductNameIsNull_ReturnsBadRequest()
        {
            // arrange


            // act
            var response = await TestClient.PostAsJsonAsync("api/products", new Product { Name = null });

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("CreateProductAsync", "")]
        public async Task CreateProductAsync_WhenProductWithActualNameExists_ReturnsBadRequest()
        {
            // arrange
            var existingProduct = ProductHelper.GetMany().First();

            // act
            var response = await TestClient.PostAsJsonAsync("api/products", existingProduct);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("UpdateProductAsync", "")]
        public async  Task UpdateProductAsync_WhenProductExists_UpdatesProductAndReturnsNoContent()
        {
            // arrange
            var getProductsResponse = await TestClient.GetAsync("api/products");
            var existingProduct = (await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>()).First();

            // act
            existingProduct.Cost = 3.99m;

            var response = await TestClient.PutAsJsonAsync($"api/products/{existingProduct.Id}", existingProduct);
            
            var getProductResponse = await TestClient.GetAsync($"api/products/{existingProduct.Id}");

            var product = await getProductResponse.Content.ReadFromJsonAsync<Product>();

            // assert
            Assert.Equal(existingProduct.Cost, product.Cost);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("UpdateProductAsync", "")]
        public async Task UpdateProductAsync_WhenProductNameIsNull_ReturnsBadRequest()
        {
            // arrange
            var getProductsResponse = await TestClient.GetAsync("api/products");
            var existingProduct = (await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>()).First();

            // act
            existingProduct.Name = null;

            var response = await TestClient.PutAsJsonAsync($"api/products/{existingProduct.Id}", existingProduct);

            // assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        [Trait("UpdateProductAsync", "")]
        public async Task UpdateProductAsync_WhenProductNotExists_ReturnsNotFound()
        {
            // arrange
          

            // act
            var response = await TestClient.PutAsJsonAsync($"api/products/{Guid.NewGuid()}", ProductHelper.GetOne());

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("ChangeProductCountAsync", "")]
        public async Task ChangeProductCountAsync_WhenProductExists_UpdatesProductCountAndReturnNoContent()
        {
            // arrange
            var getProductsResponse = await TestClient.GetAsync("api/products");
            var existingProduct = (await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>()).First();
            var valueToAdd = 200;

            // act
            existingProduct.Count += valueToAdd;

            var response = await TestClient.PutAsync($"api/products/{existingProduct.Id}_{valueToAdd}", null);

            var getProductResponse = await TestClient.GetAsync($"api/products/{existingProduct.Id}");

            var product = await getProductResponse.Content.ReadFromJsonAsync<Product>();

            // assert
            Assert.Equal(existingProduct.Count, product.Count);
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        [Trait("ChangeProductCountAsync", "")]
        public async Task ChangeProductCountAsync_WhenProductNotExists_ReturnNotFound()
        {
            // arrange


            // act
            var response = await TestClient.PutAsync($"api/products/{Guid.NewGuid()}_{200}", null);

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        [Trait("RemoveProductAsync", "")]
        public async Task RemoveProductAsync_WhenProductExists_RemovesProductAndReturnsNoContent()
        {
            // arrange
            var getProductsResponse = await TestClient.GetAsync("api/products");
            var existingProducts = await getProductsResponse.Content.ReadFromJsonAsync<List<Product>>();
            var existingProduct = existingProducts.First();

            // act
            var response = await TestClient.DeleteAsync($"api/products/{existingProduct.Id}");

            var getProductsAfterRemoveResponse = await TestClient.GetAsync("api/products");

            var existingProductsAfterRemove = await getProductsAfterRemoveResponse.Content.ReadFromJsonAsync<List<Product>>();

            // assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Equal((existingProducts.Count - 1), existingProductsAfterRemove.Count);
            Assert.DoesNotContain(existingProduct, existingProductsAfterRemove);
        }

        [Fact]
        [Trait("RemoveProductAsync", "")]
        public async Task RemoveProductAsync_WhenProductNotExists_ReturnsNotFound()
        {
            // arrange


            // act
            var response = await TestClient.DeleteAsync($"api/products/{Guid.NewGuid()}");

            // assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private int GetTotalCountOfProductsFromHeaderPagination(HttpResponseMessage response)
        {
            var pagination = response.Headers.GetValues("Pagination").First();
            var i = pagination.IndexOf(':');
            var j = pagination.IndexOf(',');
            var number = pagination.Substring((i + 1), (j - i - 1));
            
            return int.Parse(number);
        }
    }
}
