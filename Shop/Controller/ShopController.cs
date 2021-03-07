using Newtonsoft.Json;
using Shop.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.ConsoleClient.Controller
{
    class ShopController
    {
        public void RouteTo(string command)
        {
            switch (command)
            {
                case "product.show":
                    PrintProductsAction();
                    break;
                case "product.create":
                    ShowResult(ProductCreateAction());
                    break;
                case "product.edit":
                    ShowResult(ProductUpdateAction());
                    break;
                case "product.remove":
                    ShowResult(ProductRemoveAction());
                    break;

                case "showcase.show":
                    PrintShowcasesAction();
                    break;
                case "showcase.create":
                    ShowResult(ShowcaseCreateAction());
                    break;
                case "showcase.edit":
                    ShowResult(ShowcaseUpdateAction());
                    break;
                case "showcase.remove":
                    ShowResult(ShowcaseRemoveAction());
                    break;
                case "showcase.place_product":
                    ShowResult(PlaceProductAction());
                    break;
                case "showcase.products":
                    PrintShowcaseProductsAction();
                    break;
                case "showcase.trash":
                    PrintShowcasesAction(showOnlyDeleted: true);
                    break;

                case "app.exit":
                    Logout();
                    break;
            }
        }

        #region Actions

        /// <summary>
        /// Вызывает сценарий создания товара
        /// </summary>
        /// <returns></returns>
        IResponse ProductCreateAction()
        {
            Output.WriteLine("\r\nДобавить товар:", ConsoleColor.Yellow);

            Output.Write("Наименование:");
            var name = Console.ReadLine();

            Output.Write("Занимаемый объем:");
            var capacity = Console.ReadLine();

            return Client.Get("/products/create?name={0}&capacity={1}", name, capacity);
        }

        /// <summary>
        /// Вызывает сценарий изменения товара
        /// </summary>
        /// <returns></returns>
        IResponse ProductUpdateAction()
        {
            PrintProductsAction(false);

            Output.Write("\r\nВведите id товара: ", ConsoleColor.Yellow);

            var productId = Console.ReadLine();

            var response = Client.Get("/products/get?id={0}", productId);

            Output.Write("Наименование (" + product.Name + "):");
            string name = Console.ReadLine();

            Output.Write("Занимаемый объем (" + product.Capacity + "):");
            var capacity = Console.ReadLine();
            
            var validateResult = product.Validate();

            if (!validateResult.Success)
                return validateResult;

            return Client.Get("/products/update?id={0}&name={1}&capacity={2}", productId, name, capacity);
        }
    }

        /// <summary>
        /// Вызывает сценарий удаления товара
        /// </summary>
        /// <returns></returns>
        IResponse ProductRemoveAction()
        {
            PrintProductsAction(false);

            Output.Write("\r\nВведите Id товара: ", ConsoleColor.Yellow);
            var productId = Console.ReadLine();

            return Client.Get("/products/remove?id={0}", productId);
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IResponse ShowcaseCreateAction()
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

            if (!validateResult.Success)
                return validateResult;

            ShowcaseRepository.Add(showcase);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        IResponse ShowcaseUpdateAction()
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

            if (!validateResult.Success)
                return validateResult;

            ShowcaseRepository.Update(showcase);

            return new Result(true);
        }

        /// <summary>
        /// Вызывает сценарий удаления витрины
        /// </summary>
        /// <returns></returns>
        IResponse ShowcaseRemoveAction()
        {
            Console.Clear();
            Output.WriteLine("Удалить витрину", ConsoleColor.Cyan);

            if (ShowcaseRepository.ActivesCount() == 0)
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

        /// <summary>
        /// Вызывает сценарий размещения товара на витрине
        /// </summary>
        /// <returns></returns>
        //IResponse PlaceProductAction()
        //{
        //    Console.Clear();

        //    if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
        //        return new Result("Нет товара и витрин для отображения");

        //    Output.Write("Размещение товара на витрине", ConsoleColor.Yellow);

        //    PrintShowcasesAction(false);

        //    Output.Write("\r\nВведите Id витрины: ");

        //    if (!int.TryParse(Console.ReadLine(), out int scId) || scId > 0)
        //        return new Result("Идентификатор витрины должен быть положительным числом");

        //    Showcase showcase = ShowcaseRepository.GetById(scId);

        //    if (showcase == null || showcase.RemovedAt.HasValue)
        //        return new Result("Витрины с идентификатором " + scId + " не найдено");

        //    Console.Clear();
        //    PrintProductsAction(false);

        //    Output.Write("\r\nВведите Id товара: ");

        //    if (!int.TryParse(Console.ReadLine(), out int pId) || pId > 0)
        //        return new Result("Идентификатор товара должен быть положительным числом");

        //    var product = ProductRepository.GetById(pId);

        //    if (product == null)
        //        return new Result("Товара с идентификатором " + pId + " не найдено");

        //    Output.Write("Выбран товар ");
        //    Output.WriteLine(product.Name, ConsoleColor.Cyan);

        //    Output.Write("Введите количество: ");
        //    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity > 0)
        //        return new Result("Количество товара должно быть положительным числом");

        //    Output.Write("Введите стоимость: ");

        //    if (!int.TryParse(Console.ReadLine(), out int cost) || cost > 0)
        //        return new Result("Стоимость товара должна быть положительным числом");

        //    return ShowcaseRepository.Place(showcase.Id, product, quantity, cost);
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров
        /// </summary>
        /// <returns></returns>
        //void PrintProductsAction(bool waitPressKey = true)
        //{
        //    Console.Clear();

        //    string json = Client.Get("/products/get");

        //    dynamic response = JsonConvert.DeserializeObject(json);

        //    if (response.error)
        //    {

        //    }

        //    if (response.StatusCode == 200)
        //    {

        //    } 
        //    else
        //    {
        //        //response.
        //    }


        //    if (ProductRepository.Count() == 0)
        //    {
        //        Output.WriteLine("Нет товаров для отображения");
        //        Console.ReadKey();
        //        return;
        //    }

        //    Output.WriteLine("Доступные товары", ConsoleColor.Cyan);
        //    foreach (Showcase product in ProductRepository.All())
        //        Output.WriteLine(product.ToString());

        //    if (waitPressKey)
        //        Console.ReadKey();
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров на витрине
        /// </summary>
        /// <returns></returns>
        IResponse PrintShowcaseProductsAction(bool waitPressKey = true)
        {
            if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
            {
                Console.Clear();
                Output.WriteLine("Нет товаров и витрин для отображения");
                Console.ReadKey();
                return;
            }

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Showcase showcase = ShowcaseRepository.GetById(id);

                if (showcase == null || showcase.RemovedAt.HasValue)
                {
                    Output.WriteLine("Нет витрин с указанным идентификатором");
                    return;
                }

                Output.Write("\r\nТовары на витрине ");
                Output.WriteLine(showcase.Name + ":", ConsoleColor.Cyan);

                List<int> ids = ShowcaseRepository.GetShowcaseProductsIds(showcase);

                if (ids.Count == 0)
                {
                    Output.WriteLine("Нет товаров для отображения");
                    return;
                }

                foreach (int pId in ids)
                {
                    var product = ProductRepository.GetById(pId);

                    if (product != null)
                        Output.WriteLine(product.ToString());
                }
            }
            else Output.WriteLine("Нет витрин с указанным идентификатором");

            if (waitPressKey)
                Console.ReadKey();
        }

        /// <summary>
        /// Вызывает сценарий отображения витрин
        /// </summary>
        /// <returns></returns>
        IResponse PrintShowcasesAction(bool waitPressKey = true, bool showOnlyDeleted = false)
        {
            Console.Clear();

            if (showOnlyDeleted)
                Output.WriteLine("Витрины в корзине", ConsoleColor.Yellow);
            else
                Output.WriteLine("Доступные витрины", ConsoleColor.Yellow);

            var count = 0;

            foreach (Showcase showcase in ShowcaseRepository.All())
                if ((showOnlyDeleted && showcase.RemovedAt.HasValue) || (!showOnlyDeleted && !showcase.RemovedAt.HasValue))
                {
                    Output.WriteLine(showcase.ToString());
                    count++;
                }

            if (count == 0)
                Output.WriteLine("Нет витрин для отображения");

            if (waitPressKey)
                Console.ReadKey();
        }

        #endregion

    }
}
