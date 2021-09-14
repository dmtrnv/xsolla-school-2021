using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductManufacturerRepository : RepositoryBase<ProductManufacturer>, IProductManufacturerRepository
    {
        public ProductManufacturerRepository(ProductApiContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductManufacturer>> GetAllManufacturersAsync()
        {
            return await FindAll()
                .ToListAsync();
        }

        public async Task<ProductManufacturer> GetManufacturerByIdAsync(Guid id)
        {
            return await FindByCondition(m => m.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public void CreateManufacturer(ProductManufacturer manufacturer)
        {
            Create(manufacturer);
        }

        public void UpdateManufacturer(ProductManufacturer manufacturer)
        {
            Update(manufacturer);
        }

        public void DeleteManufacturer(ProductManufacturer manufacturer)
        {
            Delete(manufacturer);
        }
    }
}
