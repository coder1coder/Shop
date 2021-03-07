using Shop.Model;
using System.Net;

namespace Shop.Server.Controller
{
    internal class ProductController: Controller
    {
        internal IResponse GetResponse(string action, HttpListenerContext context)
        {
            return action switch
            {
                "" => Get(context),
                "create" => Create(context),
                "update" => Update(context),
                "delete" => Delete(context),
                _ => new Response(404)
            };
        }

        internal IResponse Get(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
                return new Response(404);

            return new Response(200, _productRepository.All());
        }

        internal IResponse Create(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST")
                return new Response(404);

            var query = context.Request.QueryString;

            if (!query.HasKeys() || (string.IsNullOrWhiteSpace(query.Get("name")) || string.IsNullOrWhiteSpace(query.Get("capacity"))))
                return new Response(400);

            if (!int.TryParse(query.Get("capacity"), out int capacity))
                return new Response(400, "bad capacity");

            var product = new Showcase
            {
                Name = query.Get("name"),
                Capacity = capacity
            };

            var validate = product.Validate();

            if (validate.IsSuccess == false)
                return new Response(400, validate.Message);

            return new Response(200, _productRepository.Add(product));
        }

        internal IResponse Update(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST")
                return new Response(404);

            var query = context.Request.QueryString;

            if (!query.HasKeys() || string.IsNullOrWhiteSpace(query.Get("id")) || !int.TryParse(query.Get("id"), out int productId))
                return new Response(400, "Идентификатор товара должен быть целым положительным числом");

            if (string.IsNullOrWhiteSpace(query.Get("name")))
                return new Response(400, "Bad request");

            var product = _productRepository.GetById(productId);

            if (product == null)
                return new Response(400, "Товар с идентификатором " + productId + " не найден");

            product.Name = query.Get("name");

            //Не даем возможность менять объем товара размещенного на витрине
            bool placedInShowcase = false;

            foreach (var showcase in _showcaseRepository.All())
                if (_showcaseRepository.GetShowcaseProductsIds(showcase).Count > 0)
                {
                    placedInShowcase = true;
                    break;
                }

            if (!placedInShowcase)
            {
                if (string.IsNullOrWhiteSpace(query.Get("capacity")) || !int.TryParse(query.Get("capacity"), out int capacityInt))
                    return new Response(400, "Bad request");

                product.Capacity = capacityInt;
            }

            var validateResult = product.Validate();

            if (!validateResult.IsSuccess)
                return new Response(400, validateResult);

            _productRepository.Update(product);
            
            return new Response(200, product);
        }

        internal IResponse Delete(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "DELETE")
                return new Response(404);

            var query = context.Request.QueryString;

            if (!query.HasKeys() || !int.TryParse(query.Get("id"), out int productId) || productId == 0)
                return new Response(400, "Bad request params");

            var product = _productRepository.GetById(productId);

            if (product == null)
                return new Response(400, "Товар с идентификатором " + productId + " не найден");

            _showcaseRepository.TakeOut(product);
            _productRepository.Remove(productId);

            return new Response(200);
        }

        internal void Seed(int count) => _productRepository.Seed(count);
    }
}
