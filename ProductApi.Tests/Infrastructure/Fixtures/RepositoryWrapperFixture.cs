using ProductApi.Data;
using ProductApi.Tests.Infrastructure.Helpers;

namespace ProductApi.Tests.Infrastructure.Fixtures
{
    public class RepositoryWrapperFixture
    {
        private ProductApiContext _context;

        public IRepositoryWrapper Create()
        {
            var mock = RepositoryWrapperHelper.GetMock(out _context);

            return mock.Object;
        }
    }
}
