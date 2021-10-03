using Moq;
using ProductApi.Data;

namespace ProductApi.Tests.Infrastructure.Helpers
{
    public static class RepositoryWrapperHelper
    {
        public static Mock<IRepositoryWrapper> GetMock(out ProductApiContext c)
        {
            var context = new DbContextHelper().Context;
            c = context;
            var repositoryWrapper = new Mock<IRepositoryWrapper>();

            repositoryWrapper.Setup(x => x.Products).Returns(new ProductRepository(context));
            repositoryWrapper.Setup(x => x.Manufacturers).Returns(new ProductManufacturerRepository(context));
            repositoryWrapper.Setup(x => x.Types).Returns(new ProductTypeRepository(context));
            repositoryWrapper.Setup(x => x.Subtypes).Returns(new ProductSubtypeRepository(context));
            repositoryWrapper.Setup(x => x.SaveAsync()).Callback(async () => await context.SaveChangesAsync());
            
            return repositoryWrapper;
        }
    }
}
