using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductTypeRepository : IRepositoryBase<ProductType>
    {
        Task<IEnumerable<ProductType>> GetAllTypesAsync();
        Task<ProductType> GetTypeByIdAsync(Guid id);
        void CreateType(ProductType type);
        void UpdateType(ProductType type);
        void DeleteType(ProductType type);
    }
}
