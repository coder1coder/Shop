using Shop.RESTApi.DAL;

namespace Shop.RESTApi.Controller
{
    internal abstract class Controller
    {
        internal static IShowcaseRepository _showcaseRepository;
        internal static IProductRepository _productRepository;

        public Controller()
        {
            _showcaseRepository = new ShowcaseRepository();
            _productRepository = new ProductRepository();
        }
    }
}
