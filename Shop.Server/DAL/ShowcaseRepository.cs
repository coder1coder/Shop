using Shop.Model;
using System;
using System.Collections.Generic;

namespace Shop.RESTApi.DAL
{
    class ShowcaseRepository : IShowcaseRepository
    {
        readonly List<Showcase> _items = new List<Showcase>();
        readonly List<ProductShowcase> _products = new List<ProductShowcase>();

        int _lastInsertedId = 0;
        int _lastProductInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Showcase> All() => _items;

        public int ActivesCount()
        {
            var count = 0;
            foreach (var item in _items)
                if (!item.RemovedAt.HasValue)
                    count++;
            return count;
        }

        public int RemovedCount()
        {
            var count = 0;
            foreach (var item in _items)
                if (item.RemovedAt.HasValue)
                    count++;
            return count;
        }

        public Showcase Add(Showcase entity)
        {
            entity.Id = ++_lastInsertedId;
            entity.CreatedAt = DateTime.Now;
            _items.Add(entity);
            return entity;
        }

        public Showcase GetById(int id)
        {
            for (int i = 0; i < _items.Count; i++)
                if (_items[i].Id.Equals(id))
                    return _items[i];

            return null;
        }

        public void Remove(int id)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id.Equals(id))
                {
                    _items[i].RemovedAt = DateTime.Now;
                    break;
                }
            }
        }

        public void Update(Showcase entity)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id.Equals(entity.Id))
                {
                    _items[i] = entity;
                    break;
                }
            }
        }

        public void Seed(int count)
        {
            for (var i = 0; i < count; i++)
            {
                _items.Add(new Showcase()
                {
                    Id = ++_lastInsertedId,
                    Name = "Витрина " + (i + 1),
                    CreatedAt = DateTime.Now,
                    MaxCapacity = 1 + i
                });
            }
        }

        public IResponse Place(int showcaseId, Showcase product, int quantity, decimal cost)
        {
            var ps = new ProductShowcase(showcaseId, product.Id, quantity, cost)
            {
                Id = ++_lastProductInsertedId
            };

            var validate = ps.Validate();

            if (validate.IsSuccess == false)
                return new Response(400, validate.Message);

            return new Response(200);
        }

        public List<int> GetShowcaseProductsIds(Showcase showcase)
        {
            var ids = new List<int>();
            foreach (var psc in _products)
                if (showcase.Id == psc.ShowcaseId)
                    ids.Add(psc.ProductId);

            return ids;
        }
        public void TakeOut(Showcase product)
        {
            for (var i = 0; i < _products.Count; i++)
            {
                if (_products[i].ProductId.Equals(product.Id))
                {
                    _products.RemoveAt(i);
                    break;
                }
            }
        }

        public List<ProductShowcase> GetShowcaseProducts(Showcase showcase)
        {
            var result = new List<ProductShowcase>();
            foreach (var psc in _products)
                if (showcase.Id == psc.ShowcaseId)
                    result.Add(psc);

            return result;
        }
    }
}
