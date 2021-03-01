using Shop.Model;
using Shop.Server.DAL;
using System;
using System.Net;

namespace Shop.Server.Controller
{
    public class ShowcaseController
    {
        private readonly IShowcaseRepository repository;

        public ShowcaseController()
        {
            repository = new ShowcaseRepository();
        }

        public void Seed(int count) => repository.Seed(count);

        internal object RouteToAction(string action, HttpListenerRequest request)
        {
            return action switch
            {
                "" => Get(),
                "create" => Create(request),
                "update" => Update(request),
                "delete" => Delete(request),
                "place"  => Place(request),
                "products" => GetProducts(request),
                _ => new Result() { Message = "404" },
            };
        }

        private object Get()
        {
            throw new NotImplementedException();
        }

        private object Place(HttpListenerRequest request)
        {
            throw new NotImplementedException();
        }

        private object GetProducts(HttpListenerRequest request)
        {
            throw new NotImplementedException();
        }

        private object Delete(HttpListenerRequest request)
        {
            throw new NotImplementedException();
        }

        private object Update(HttpListenerRequest request)
        {
            throw new NotImplementedException();
        }

        private object Create(HttpListenerRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
