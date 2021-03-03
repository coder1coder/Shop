using Shop.Model;
using System;
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

        private IResponse Get(HttpListenerContext context)
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

            var product = new Product
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

            if (!query.HasKeys() || (string.IsNullOrWhiteSpace(query.Get("id")) || string.IsNullOrWhiteSpace(query.Get("name"))))

                if (!int.TryParse(Console.ReadLine(), out int pid))
                return new Response(400, "Идентификатор должен быть целым положительным числом");

            var product = _productRepository.GetById(pid);

            if (product == null)
                return new Response(400, "Товар с идентификатором " + pid + " не найден");

            Output.Write("Наименование (" + product.Name + "):");
            string name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
                product.Name = name;

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
                Output.Write("Занимаемый объем (" + product.Capacity + "):");

                if (int.TryParse(Console.ReadLine(), out int capacityInt))
                    product.Capacity = capacityInt;
            }
            else Output.WriteLine("Нельзя изменить объем товара, размещенного на витрине", ConsoleColor.Yellow);

            var validateResult = product.Validate();

            if (!validateResult.IsSuccess)
                return validateResult;

            _productRepository.Update(product);
            result.IsSuccess = true;


            return result;
        }

        internal IResponse Delete(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "DELETE")
                return new Response() { Message = "404" };

            var query = context.Request.QueryString;

            if (!query.HasKeys() || !int.TryParse(query.Get("id"), out int id) || id == 0)
                return new Response() { Message = "fail data" };

            var product = _productRepository.GetById(id);

            if (product == null)
                return new Result("Товар с идентификатором " + id + " не найден");

            _showcaseRepository.TakeOut(product);
            _productRepository.Remove(id);

            return new Result(true);
        }

        internal void Seed(int count) => _productRepository.Seed(count);
    }
}
