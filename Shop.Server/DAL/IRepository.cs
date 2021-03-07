using System.Collections.Generic;

namespace Shop.RESTApi.DAL
{
    interface IRepository<T>
    {
        T Add(T entity);
        void Update(T entity);
        void Remove(int id);
        T GetById(int id);
        IEnumerable<T> All();
        int Count();
        void Seed(int count);
    }
}
