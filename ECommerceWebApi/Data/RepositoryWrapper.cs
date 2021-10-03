using System.Threading.Tasks;

namespace ProductApi.Data
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly ProductApiContext _context;
        private IProductRepository _products;
        private IProductManufacturerRepository _manufacturers;
        private IProductTypeRepository _types;
        private IProductSubtypeRepository _subtypes;

        public IProductRepository Products
        {
            get
            {
                if (_products == null)
                {
                    _products = new ProductRepository(_context);
                }

                return _products;
            }
        }

        public IProductManufacturerRepository Manufacturers
        {
            get
            {
                if (_manufacturers == null)
                {
                    _manufacturers = new ProductManufacturerRepository(_context);
                }

                return _manufacturers;
            }
        }

        public IProductTypeRepository Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new ProductTypeRepository(_context);
                }

                return _types;
            }
        }

        public IProductSubtypeRepository Subtypes
        {
            get
            {
                if (_subtypes == null)
                {
                    _subtypes = new ProductSubtypeRepository(_context);
                }

                return _subtypes;
            }
        }

        public RepositoryWrapper(ProductApiContext context)
        {
            _context = context;
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
