﻿using Shop.Model;
using System.Collections.Generic;

namespace Shop.Simple.DAL
{
    internal interface IShowcaseRepository: IRepository<Showcase>
    {
        int ActivesCount();
        int RemovedCount();

        void TakeOut(Product product);
        IEnumerable<int> GetShowcaseProductsIds(Showcase showcase);
        IEnumerable<ProductShowcase> GetShowcaseProducts(Showcase showcase);

        IResult Place(int showcaseId, Product product, int quantity, decimal cost);
    }
}