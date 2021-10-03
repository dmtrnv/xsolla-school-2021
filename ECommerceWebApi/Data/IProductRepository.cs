using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductApi.Misc;
using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync();
        IQueryable<Product> GetProductsFilteredByCostWithDetails(ProductParameters parameters);
        IQueryable<Product> GetProductsFilteredByCostAndTypeNameWithDetails(ProductParameters parameters);
        Task<Product> GetProductByIdAsync(Guid id);
        Task<Product> GetProductByIdWithDetailsAsync(Guid id);
        Task<Product> GetProductBySkuAsync(string sku);
        Task<Product> GetProductBySkuWithDetailsAsync(string sku);
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        void DeleteProduct(Product product);
    }
}
