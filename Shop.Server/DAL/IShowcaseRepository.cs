using Shop.Model;
using Shop.Server.Model;
using System.Collections.Generic;

namespace Shop.Server.DAL
{
    internal interface IShowcaseRepository: IRepository<Showcase>
    {
        int ActivesCount();
        int RemovedCount();

        void TakeOut(Showcase product);
        List<int> GetShowcaseProductsIds(Showcase showcase);
        List<ProductShowcase> GetShowcaseProducts(Showcase showcase);

        IResponse Place(int showcaseId, Showcase product, int quantity, decimal cost);
    }
}