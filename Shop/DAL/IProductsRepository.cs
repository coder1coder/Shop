using Shop.Model;
using System.Collections.Generic;

namespace Shop.Simple.DAL
{
    interface IProductRepository : IRepository<Product>
    {
        int ProductsCapacity(IEnumerable<ProductShowcase> productsShowcase);
    }
}
