using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductApi.Models;

namespace ProductApi.Data
{
    public interface IProductManufacturerRepository : IRepositoryBase<ProductManufacturer>
    {
        Task<IEnumerable<ProductManufacturer>> GetAllManufacturersAsync();
        Task<ProductManufacturer> GetManufacturerByIdAsync(Guid id);
        void CreateManufacturer(ProductManufacturer manufacturer);
        void UpdateManufacturer(ProductManufacturer manufacturer);
        void DeleteManufacturer(ProductManufacturer manufacturer);
    }
}
