using Shop.Model;
using Shop.Server.Model;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Shop.Server.Controller
{
    internal class ShowcaseController: Controller
    {
        internal IResponse GetResponse(string action, HttpListenerContext context)
        {
            return action switch
            {
                "" => Get(context),
                "create" => Create(context),
                "update" => Update(context),
                "delete" => Delete(context),
                "place" => Place(context),
                "products" => GetPlacementProducts(context),
                _ => new Response(404)
            };
        }

        internal IResponse Get(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
                return new Response(404);

            var query = context.Request.QueryString;

            if (query.HasKeys() && !string.IsNullOrWhiteSpace(query.Get("id")) && int.TryParse(query.Get("id"), out int showcaseId))
                return new Response(200, _showcaseRepository.GetById(showcaseId));
            else
                return new Response(200, _showcaseRepository.All().Where(x => x.RemovedAt.HasValue == false));
        }

        internal IResponse Create(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST")
                return new Response(404);

            var query = context.Request.QueryString;

            if (!query.HasKeys())
                return new Response(400, "Bad request");

            if (string.IsNullOrWhiteSpace(query.Get("name")) || string.IsNullOrWhiteSpace(query.Get("max_capacity")))
                return new Response(400, "Bad request params");

            if (!int.TryParse(query.Get("max_capacity"), out int maxCapacity))
                return new Response(400, "Bad request params");

            var showcase = new Showcase
            {
                Name = query.Get("name"),
                MaxCapacity = maxCapacity
            };

            var validateResult = showcase.Validate();

            if (!validateResult.IsSuccess)
                return new Response(400, validateResult);

            return new Response(200, _showcaseRepository.Add(showcase));
        }

        internal IResponse Update(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST")
                return new Response(404);

            var query = context.Request.QueryString;

            if (!query.HasKeys())
                return new Response(400, "Bad request params");

            if (string.IsNullOrWhiteSpace(query.Get("id")) || !int.TryParse(query.Get("id"), out int showcaseId))
                return new Response(400, "Bad request params");

            var showcase = _showcaseRepository.GetById(showcaseId);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Response(400, "Bad request params");

            if (string.IsNullOrWhiteSpace(query.Get("name")))
                return new Response(400, "Bad request params");

            showcase.Name = query.Get("name");

            if (!string.IsNullOrWhiteSpace(query.Get("max_capacity")))
            {
                 if (!int.TryParse(query.Get("max_capacity"), out int maxCapacity))
                    return new Response(400, "Bad request params");

                //Чекаем изменение объема в меньшую сторону
                var productShowcases = _showcaseRepository.GetShowcaseProducts(showcase);
                int showcaseFullness = _productRepository.ProductsCapacity(productShowcases);

                if (showcaseFullness > maxCapacity)
                    return new Response(400, "Невозможно установить заданный объем, объем размещенного товара превышеает значение");

                showcase.MaxCapacity = maxCapacity;
            }

            var validateResult = showcase.Validate();

            if (!validateResult.IsSuccess)
                return new Response(400, validateResult);

            _showcaseRepository.Update(showcase);

            return new Response(200, showcase);
        }

        internal IResponse Delete(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "DELETE")
                return new Response(404);

            var query = context.Request.QueryString;

            if (query.HasKeys() == false)
                return new Response(400, "Bad request params");

            if (string.IsNullOrWhiteSpace(query.Get("id")) || !int.TryParse(query.Get("id"), out int showcaseId))
                return new Response(400, "Bad showcase id");

            var showcase = _showcaseRepository.GetById(showcaseId);

            if (showcase == null)
                return new Response(400, "Bad showcase id");

            if (_showcaseRepository.GetShowcaseProductsIds(showcase).Count != 0)
                return new Response(400, "Невозможно удалить витрину, на которой размещены товары");

            _showcaseRepository.Remove(showcase.Id);
            return new Response(200);
        }

        internal IResponse Place(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "POST")
                return new Response(404);

            var query = context.Request.QueryString;

            if (query.HasKeys() == false)
                return new Response(400, "Bad request params");

            if (!int.TryParse(query.Get("showcase_id"), out int showcaseId) || showcaseId < 1)
                return new Response(400, "Bad showcase id");

            var showcase = _showcaseRepository.GetById(showcaseId);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Response(400, "Витрины с идентификатором " + showcaseId + " не найдено");

            if (!int.TryParse(query.Get("product_id"), out int productId) || productId < 1)
                return new Response(400, "Идентификатор товара должен быть положительным числом");

            var product = _productRepository.GetById(productId);

            if (product == null)
                return new Response(400, "Товара с идентификатором " + productId + " не найдено");

            if (!int.TryParse(query.Get("quantity"), out int quantity) || quantity < 1)
                return new Response(400, "Количество товара должно быть положительным числом");

            if (!int.TryParse(query.Get("cost"), out int cost) || cost < 1)
                return new Response(400, "Стоимость товара должна быть положительным числом");

            return _showcaseRepository.Place(showcaseId, product, quantity, cost);
        }

        internal IResponse GetPlacementProducts(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
                return new Response(404);

            var data = context.Request.QueryString;

            if (data.HasKeys() == false)
                return new Response(400, "Bad request params");

            if (!int.TryParse(data.Get("id"), out int showcaseId) || showcaseId < 1)
                return new Response(400, "Bad request params");

            var showcase = _showcaseRepository.GetById(showcaseId);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Response(400, "Нет витрин с указанным идентификатором");

            var products_ids = _showcaseRepository.GetShowcaseProductsIds(showcase);

            var list = new List<Product>();

            foreach (int productId in products_ids)
            {
                var product = _productRepository.GetById(productId);
                if (product != null)
                    list.Add(product);
            }

            return new Response(200, list);
        }

        internal void Seed(int count) => _showcaseRepository.Seed(count);
    }
}
