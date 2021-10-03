using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Misc;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ProductApiContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await FindAll()
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithDetailsAsync()
        {
            return await FindAll()
                .Include(p => p.Manufacturer)
                .Include(p => p.Type)
                .Include(p => p.Subtype)
                .ToListAsync();
        }

        public IQueryable<Product> GetProductsFilteredByCostWithDetails(ProductParameters parameters)
        {
            return FindByCondition(p => (p.Cost >= parameters.MinCost) && (p.Cost <= parameters.MaxCost))
                .Include(p => p.Manufacturer)
                .Include(p => p.Type)
                .Include(p => p.Subtype)
                .OrderBy(p => p.Name);
        }
        
        public IQueryable<Product> GetProductsFilteredByCostAndTypeNameWithDetails(ProductParameters parameters)
        {
            return FindByCondition(p => (p.Cost >= parameters.MinCost) && (p.Cost <= parameters.MaxCost))
                .Include(p => p.Type)
                .Where(p => p.Type.Name == parameters.TypeName)
                .Include(p => p.Manufacturer)
                .Include(p => p.Subtype)
                .OrderBy(p => p.Name);
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            return await FindByCondition(p => p.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductByIdWithDetailsAsync(Guid id)
        {
            return await FindByCondition(p => p.Id.Equals(id))
                .Include(p => p.Manufacturer)
                .Include(p => p.Type)
                .Include(p => p.Subtype)
                .FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductBySkuAsync(string sku)
        {
            return await FindByCondition(p => p.Sku.Equals(sku))
                .FirstOrDefaultAsync();
        }

        public async Task<Product> GetProductBySkuWithDetailsAsync(string sku)
        {
            return await FindByCondition(p => p.Sku.Equals(sku))
                .Include(p => p.Manufacturer)
                .Include(p => p.Type)
                .Include(p => p.Subtype)
                .FirstOrDefaultAsync();
        }

        // Here method's parameter "product" contains only fields that available in ProductCreateUpdateDto.
        public async Task CreateProductAsync(Product product)
        {
            await SetProductManufacturerCorrect(product);
            await SetProductTypeCorrect(product);
            await SetProductSubtypeCorrect(product);

            product.UpdateSku();

            Create(product);
        }

        // Here method's parameter "product" contains only fields that available in ProductCreateUpdateDto.
        public async Task UpdateProductAsync(Product product)
        {
            product.ManufacturerId = Guid.Empty;
            product.Manufacturer.Id = Guid.Empty;
            product.TypeId = Guid.Empty;
            product.Type.Id = Guid.Empty;
            product.Type.Code = default;
            product.SubtypeId = Guid.Empty;
            product.Subtype.Id = Guid.Empty;
            product.Subtype.Code = default;

            await SetProductManufacturerCorrect(product);
            await SetProductTypeCorrect(product);
            await SetProductSubtypeCorrect(product);

            product.UpdateSku();

            Update(product);
        }

        public void DeleteProduct(Product product)
        {
            Delete(product);
        }

        private async Task SetProductManufacturerCorrect(Product product)
        {
            if ((await Context.Manufacturers.FirstOrDefaultAsync(m => m.Name == product.Manufacturer.Name)) == null)
            {
                await Context.Manufacturers.AddAsync(product.Manufacturer);
                await Context.SaveChangesAsync();
            }

            product.Manufacturer = await Context.Manufacturers.FirstOrDefaultAsync(m => m.Name == product.Manufacturer.Name);
            product.ManufacturerId = product.Manufacturer.Id;
        }

        private async Task SetProductTypeCorrect(Product product)
        {
            if ((await Context.Types.FirstOrDefaultAsync(t => t.Name == product.Type.Name)) == null)
            {
                await Context.Types.AddAsync(product.Type);
                await Context.SaveChangesAsync();
            }

            product.Type = await Context.Types.FirstOrDefaultAsync(t => t.Name == product.Type.Name);
            product.TypeId = product.Type.Id;
        }

        private async Task SetProductSubtypeCorrect(Product product)
        {
            if ((await Context.Subtypes.FirstOrDefaultAsync(s => s.Name == product.Subtype.Name)) == null)
            {
                await Context.Subtypes.AddAsync(product.Subtype);
                await Context.SaveChangesAsync();
            }

            product.Subtype = await Context.Subtypes.FirstOrDefaultAsync(s => s.Name == product.Subtype.Name);
            product.SubtypeId = product.Subtype.Id;
        }
    }
}
