﻿using Shop.Model;
using System.Collections.Generic;

namespace Shop.Simple.DAL
{
    class ProductRepository : IProductRepository
    {
        private readonly List<Product> _items = new List<Product>();
        int _lastInsertedId = 0;

        public int Count() => _items.Count;
        public IEnumerable<Product> All() => _items;

        public void Add(Product entity)
        {
            entity.Id = ++_lastInsertedId;
            _items.Add(entity);
        }

        public Product GetById(int id) => _items.Find(x => x.Id == id);

        public void Remove(int id)
        {
            for (var i = 0; i < _items.Count; i++)
                if (_items[i].Id.Equals(id))
                {
                    _items.RemoveAt(i);
                    break;
                }
        }

        public void Update(Product entity)
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
                _items.Add(new Product()
                {
                    Id = ++_lastInsertedId,
                    Name = "Товар " + (i + 1),
                    Capacity = 1 + i
                });
            }
        }

        public int ProductsCapacity(IEnumerable<ProductShowcase> productsShowcase)
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
