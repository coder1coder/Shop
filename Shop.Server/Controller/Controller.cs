using Shop.Server.DAL;

namespace Shop.Server.Controller
{
    internal abstract class Controller
    {
        internal readonly static IShowcaseRepository _showcaseRepository = new ShowcaseRepository();
        internal static IProductRepository _productRepository;

        public Controller()
        {
            _productRepository = new ProductRepository();
        }
    }
}
