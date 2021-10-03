using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductSubtypeRepository : IRepositoryBase<ProductSubtype>
    {
        Task<IEnumerable<ProductSubtype>> GetAllSubtypesAsync();
        Task<ProductSubtype> GetSubtypeById(Guid id);
        void CreateSubtype(ProductSubtype subtype);
        void UpdateSubtype(ProductSubtype subtype);
        void DeleteSubtype(ProductSubtype subtype);
    }
}
