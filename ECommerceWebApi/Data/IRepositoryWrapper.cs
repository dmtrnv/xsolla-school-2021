using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Data
{
    public interface IRepositoryWrapper
    {
        IProductRepository Products { get; }
        IProductManufacturerRepository Manufacturers { get; }
        IProductTypeRepository Types { get; }
        IProductSubtypeRepository Subtypes { get; }
        Task SaveAsync();
    }
}
