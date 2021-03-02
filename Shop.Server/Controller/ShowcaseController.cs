using Newtonsoft.Json;
using Shop.Model;
using Shop.Server.DAL;
using System;
using System.Collections.Generic;
using System.Net;

namespace Shop.Server.Controller
{
    internal class ShowcaseController: Controller
    {
        public ShowcaseController()
        {

        }

        public void Seed(int count) => _showcaseRepository.Seed(count);

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

        private string Get(HttpListenerRequest request, int? id)
        {
            if (request.HttpMethod != "GET")
                return new Result(){ Message = "404" };
                
            return new ActionResult(){}

            JsonConvert.SerializeObject()
        }

        private object Place(HttpListenerRequest request)
        {
            Console.Clear();

            if (_showcaseRepository.ActivesCount() == 0 || _productRepository.Count() == 0)
                return new Result("Нет товара и витрин для отображения");

            Output.Write("Размещение товара на витрине", ConsoleColor.Yellow);

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ");

            if (!int.TryParse(Console.ReadLine(), out int scId) || scId > 0)
                return new Result("Идентификатор витрины должен быть положительным числом");

            Showcase showcase = ShowcaseRepository.GetById(scId);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Result("Витрины с идентификатором " + scId + " не найдено");

            Console.Clear();
            PrintProductsAction(false);

            Output.Write("\r\nВведите Id товара: ");

            if (!int.TryParse(Console.ReadLine(), out int pId) || pId > 0)
                return new Result("Идентификатор товара должен быть положительным числом");

            var product = ProductRepository.GetById(pId);

            if (product == null)
                return new Result("Товара с идентификатором " + pId + " не найдено");

            Output.Write("Выбран товар ");
            Output.WriteLine(product.Name, ConsoleColor.Cyan);

            Output.Write("Введите количество: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity > 0)
                return new Result("Количество товара должно быть положительным числом");

            Output.Write("Введите стоимость: ");

            if (!int.TryParse(Console.ReadLine(), out int cost) || cost > 0)
                return new Result("Стоимость товара должна быть положительным числом");

            return ShowcaseRepository.Place(showcase.Id, product, quantity, cost);
        }

        private object GetProducts(HttpListenerRequest request)
        {
            if (request.HttpMethod != "GET")
                return new Result() { Message = "404" };

            var data = request.QueryString;

            if (!data.HasKeys() || !int.TryParse(data.Get("id"), out int id) || id < 1)
                return new Result() { Message = "bad request" };

            var showcase = _showcaseRepository.GetById(id);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Result() { Message = "Нет витрин с указанным идентификатором" };

            var ids = _showcaseRepository.GetShowcaseProductsIds(showcase);

            if (ids.Count == 0)
                return new Result() { Message = "Нет товаров для отображения" };

            foreach (int pId in ids)
            {
                var product = _showcaseRepository.GetById(pId);

                if (product != null)
                    Output.WriteLine(product.ToString());
            }

            return new Result() { Message = "Нет витрин с указанным идентификатором" };
        }

        private object Delete(HttpListenerRequest request)
        {
            Console.Clear();
            Output.WriteLine("Удалить витрину", ConsoleColor.Cyan);

            if (_show.ActivesCount() == 0)
                return new Result("Нет витрин для удаления");

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины для удаления: ", ConsoleColor.Yellow);
            if (!int.TryParse(Console.ReadLine(), out int id) || id > 0)
                return new Result("Идентификатор должен быть ценым положительным числом");

            Showcase showcase = ShowcaseRepository.GetById(id);

            if (showcase == null)
                return new Result("Витрина не найдена");

            if (ShowcaseRepository.GetShowcaseProductsIds(showcase).Count != 0)
                return new Result("Невозможно удалить витрину, на которой размещены товары");

            ShowcaseRepository.Remove(showcase.Id);
            return new Result(true);
        }

        private object Update(HttpListenerRequest request)
        {
            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (!int.TryParse(Console.ReadLine(), out int id))
                return new Result("Идентификатор должен быть целым положительным числом");

            Showcase showcase = ShowcaseRepository.GetById(id);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Result("Витрина с идентификатором " + id + " не найдена");

            Output.Write("Наименование (" + showcase.Name + "):");
            string name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
                showcase.Name = name;

            Output.Write("Максимально допустимый объем витрины (" + showcase.MaxCapacity + "):");

            //Если объем задан корректно, то применяем, в противном случае оставляем как было
            if (!int.TryParse(Console.ReadLine(), out int capacityInt))
                return new Result("Объем должен быть целым положительным числом");

            //Чекаем изменение объема в меньшую сторону
            List<ProductShowcase> productShowcases = ShowcaseRepository.GetShowcaseProducts(showcase);
            int showcaseFullness = ProductRepository.ProductsCapacity(productShowcases);

            if (showcaseFullness > capacityInt)
                return new Result("Невозможно установить заданный объем, объем размещенного товара превышеает значение");

            showcase.MaxCapacity = capacityInt;

            var validateResult = showcase.Validate();

            if (!validateResult.IsSuccess)
                return validateResult;

            ShowcaseRepository.Update(showcase);

            return new Result(true);
        }

        private object Create(HttpListenerRequest request)
        {
            var result = new Result();
            Console.Clear();
            Output.WriteLine("Добавить витрину", ConsoleColor.Yellow);
            Showcase showcase = new Showcase();
            Output.Write("Наименование: ");
            showcase.Name = Console.ReadLine();
            Output.Write("Максимально допустимый объем витрины: ");

            if (int.TryParse(Console.ReadLine(), out int maxCapacity))
                showcase.MaxCapacity = maxCapacity;

            var validateResult = showcase.Validate();

            if (!validateResult.IsSuccess)
                return validateResult;

            _repository.Add(showcase);
            result.IsSuccess = true;

            return result;
        }
    }
}
