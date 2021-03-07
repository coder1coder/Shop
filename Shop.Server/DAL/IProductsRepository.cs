using Shop.Model;
using System.Collections.Generic;

namespace Shop.RESTApi.DAL
{
    interface IProductRepository : IRepository<Showcase>
    {
        int ProductsCapacity(List<ProductShowcase> productsShowcase);
    }
}
