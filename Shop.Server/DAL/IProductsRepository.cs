using Shop.Model;
using System.Collections.Generic;

namespace Shop.Server.DAL
{
    interface IProductRepository : IRepository<Showcase>
    {
        int ProductsCapacity(List<ProductShowcase> productsShowcase);
    }
}
