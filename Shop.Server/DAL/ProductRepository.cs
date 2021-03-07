using Shop.Model;
using System.Collections.Generic;

namespace Shop.RESTApi.DAL
{
    class ProductRepository : IProductRepository
    {
        readonly List<Showcase> _items = new List<Showcase>();
        int _lastInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Showcase> All() => _items;

        public Showcase Add(Showcase entity)
        {
            entity.Id = ++_lastInsertedId;
            _items.Add(entity);
            return entity;
        }

        public Showcase GetById(int id)
        {
            for (var i = 0; i < _items.Count; i++)
                if (_items[i].Id.Equals(id))
                    return _items[i];

            return null;
        }

        public void Remove(int id)
        {
            for (var i = 0; i < _items.Count; i++)
                if (_items[i].Id.Equals(id))
                {
                    _items.RemoveAt(i);
                    break;
                }
        }

        public void Update(Showcase entity)
        {
            for (var i = 0; i < _items.Count; i++)
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
                    Name = "Товар " + (i + 1),
                    Capacity = 1 + i
                });
            }
        }

        public int ProductsCapacity(List<ProductShowcase> productsShowcase)
        {
            var capacity = 0;

            foreach (var item in productsShowcase)
                foreach (var product in _items)
                    if (product.Id == item.ProductId)
                        capacity += item.Quantity * product.Capacity;

            return capacity;
        }
    }
}
