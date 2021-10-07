using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Misc;
using ProductApi.Tests.Infrastructure.Helpers;

namespace ProductApi.IntegrationTests
{
    public class IntegrationTest
    {
        protected readonly HttpClient TestClient;

        protected IntegrationTest()
        {
            var appFactory = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ProductApiContext>));
                        
                        services.Remove(dbContextDescriptor);

                        services.AddDbContext<ProductApiContext>(options =>
                        {
                            options.UseInMemoryDatabase("integration_tests");
                        });
                    });
                });

            var context = appFactory.Services.CreateScope().ServiceProvider.GetService<ProductApiContext>();

            context.Database.EnsureDeleted();
            
            context.Products.AddRange(ProductHelper.GetMany());

            context.SaveChanges();

            foreach (var product in context.Products)
            {
                product.UpdateSku();
                context.Products.Update(product);
            }

            context.SaveChanges();

            TestClient = appFactory.CreateClient();
        }
    }
}
