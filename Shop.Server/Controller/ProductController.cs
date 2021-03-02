using Shop.Model;
using Shop.Server.DAL;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shop.Server.Controller
{
    internal class ProductController: Controller
    {
        internal IResult RouteToAction(string action, HttpListenerContext context)
        {
            return action switch
            {
                "" => Get(context),
                "create" => Create(context),
                "update" => Update(context),
                "delete" => Delete(context),
                _ => new Result() { Message = "404" },
            };
        }

        internal string Get(HttpListenerContext context)
        {
            if (context.Request.HttpMethod != "GET")
                return context.Response.OutputStream

            return _productRepository.All();
        }

        internal IResult Create(HttpListenerContext context)
        {
            var data = request.QueryString;

            if (!data.HasKeys() || (string.IsNullOrWhiteSpace(data.Get("name")) || string.IsNullOrWhiteSpace(data.Get("capacity"))))
                return new Result() { Message = "fail data" };

            if (!int.TryParse(data.Get("capacity"), out int capacity))
                return new Result() { Message = "fail data" };

            var product = new Product
            {
                Name = data.Get("name"),
                Capacity = capacity
            };

            if (!product.Validate().IsSuccess)
                return new Result() { Message = "fail validate" };

            _productRepository.Add(product);

            return new Result(true);
        }

        internal IResult Update(HttpListenerRequest request)
        {
            var result = new Result();

            Output.Write("\r\nВведите id товара: ", ConsoleColor.Yellow);

            if (!int.TryParse(Console.ReadLine(), out int pid))
                return new Result("Идентификатор должен быть целым положительным числом");

            var product = _productRepository.GetById(pid);

            if (product == null)
                return new Result("Товар с идентификатором " + pid + " не найден");

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

        internal IResult Delete(HttpListenerRequest request)
        {
            if (request.HttpMethod != "DELETE")
                return new Result() { Message = "404" };

            var query = request.QueryString;

            if (!query.HasKeys() || !int.TryParse(query.Get("id"), out int id) || id == 0)
                return new Result() { Message = "fail data" };

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
