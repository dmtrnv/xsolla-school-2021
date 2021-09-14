using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductApi.Models;

namespace ProductApi.Data
{
    public class ProductSubtypeRepository : RepositoryBase<ProductSubtype>, IProductSubtypeRepository
    {
        public ProductSubtypeRepository(ProductApiContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductSubtype>> GetAllSubtypesAsync()
        {
            return await FindAll()
                .ToListAsync();
        }

        public async Task<ProductSubtype> GetSubtypeById(Guid id)
        {
            return await FindByCondition(s => s.Id.Equals(id))
                .FirstOrDefaultAsync();
        }

        public void CreateSubtype(ProductSubtype subtype)
        {
            Create(subtype);
        }

        public void UpdateSubtype(ProductSubtype subtype)
        {
            Update(subtype);
        }

        public void DeleteSubtype(ProductSubtype subtype)
        {
            Delete(subtype);
        }
    }
}
