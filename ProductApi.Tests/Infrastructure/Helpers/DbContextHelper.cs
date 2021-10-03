using System.Linq;
using Microsoft.EntityFrameworkCore;
using ProductApi.Data;
using ProductApi.Misc;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public class DbContextHelper
    {
        public ProductApiContext Context { get; set; }
        
        public DbContextHelper()
        {
            var builder = new DbContextOptionsBuilder<ProductApiContext>();
            builder.UseInMemoryDatabase("unit_testing");
            
            var options = builder.Options;
            Context = new ProductApiContext(options);
            Context.Database.EnsureDeleted();

            Context.Manufacturers.AddRange(ManufacturerHelper.GetMany());
            Context.Types.AddRange(TypeHelper.GetMany());
            Context.Subtypes.AddRange(SubtypeHelper.GetMany());
            Context.SaveChanges();

            var products = ProductHelper.GetMany().ToList();
            foreach (var product in products)
            {
                var manufacturer = Context.Manufacturers.FirstOrDefault(m => (m.Name == product.Manufacturer.Name) && (m.Abbreviation == product.Manufacturer.Abbreviation));
                if (manufacturer != null)
                {
                    product.Manufacturer = null;
                    product.ManufacturerId = manufacturer.Id;
                }

                var type = Context.Types.FirstOrDefault(t => t.Name == product.Type.Name);
                if (type != null)
                {
                    product.Type = null;
                    product.TypeId = type.Id;
                }

                var subtype = Context.Subtypes.FirstOrDefault(s => s.Name == product.Subtype.Name);
                if (subtype != null)
                {
                    product.Subtype = null;
                    product.SubtypeId = subtype.Id;
                }

                Context.Products.Add(product);
            }

            Context.SaveChanges();

            foreach (var product in Context.Products)
            {
                product.UpdateSku();
                Context.Products.Update(product);
            }
            
            Context.SaveChanges();
        }
    }
}
