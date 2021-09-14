using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductTypeRepository : RepositoryBase<ProductType>, IProductTypeRepository
    {
        public ProductTypeRepository(ProductApiContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductType>> GetAllTypesAsync()
        {
            return await FindAll()
                .ToListAsync();
        }

        public async Task<ProductType> GetTypeByIdAsync(Guid id)
        {
            return await FindByCondition(t => t.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public void CreateType(ProductType type)
        {
            Create(type);
        }

        public void UpdateType(ProductType type)
        {
            Update(type);
        }

        public void DeleteType(ProductType type)
        {
            Delete(type);
        }
    }
}
