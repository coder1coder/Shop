using Shop.Model;
using Shop.Simple.DAL;
using System;
using System.Linq;

namespace Shop.Simple.Controller
{
    class ShopController
    {
        public bool IsLoggedIn { get; private set; }

        private IProductRepository ProductRepository { get; set; }
        private IShowcaseRepository ShowcaseRepository { get; set; }

        public void Login() => IsLoggedIn = true;
        public void Logout() => IsLoggedIn = false;

        public ShopController()
        {
            ProductRepository = new ProductRepository();
            ProductRepository.Seed(2);

            ShowcaseRepository = new ShowcaseRepository();
            ShowcaseRepository.Seed(2);
        }

        #region Actions

        /// <summary>
        /// Вызывает сценарий создания товара
        /// </summary>
        /// <returns></returns>
        internal IResult ProductCreateAction()
        {
            var result = new Result();
            Output.WriteLine("\r\nДобавить товар:", ConsoleColor.Yellow);
            var p = new Product();
            Output.Write("Наименование:");
            p.Name = Console.ReadLine();
            Output.Write("Занимаемый объем:");

            if (int.TryParse(Console.ReadLine(), out int capacity))
                p.Capacity = capacity;

            var validateResult = p.Validate();

            if (!validateResult.IsSuccess)
                return new Result(validateResult.Message);

            ProductRepository.Add(p);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий изменения товара
        /// </summary>
        /// <returns></returns>
        internal IResult ProductUpdateAction()
        {
            PrintProductsAction();

            var result = new Result();

            Output.Write("\r\nВведите id товара: ", ConsoleColor.Yellow);

            if (!int.TryParse(Console.ReadLine(), out int pid))
                return new Result("Идентификатор должен быть целым положительным числом");
            
            var product = ProductRepository.GetById(pid);

            if (product == null)
                return new Result("Товар с идентификатором " + pid + " не найден");

            Output.Write("Наименование (" + product.Name + "):");
            var name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
                product.Name = name;

            //Не даем возможность менять объем товара размещенного на витрине
            bool placedInShowcase = false;

            foreach (var showcase in ShowcaseRepository.All())
                if (Enumerable.Count(ShowcaseRepository.GetShowcaseProductsIds(showcase)) > 0)
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
                return new Result(validateResult.Message);

            ProductRepository.Update(product);
            result.Success = true;
            

            return result;
        }

        /// <summary>
        /// Вызывает сценарий удаления товара
        /// </summary>
        /// <returns></returns>
        internal IResult ProductRemoveAction()
        {
            PrintProductsAction();

            Output.Write("\r\nВведите Id товара: ", ConsoleColor.Yellow);
            if (!int.TryParse(Console.ReadLine(), out int id) || id < 1)
                return new Result("Идентификатор должен быть целым положительным числом");

            var product = ProductRepository.GetById(id);

            if (product == null)
                return new Result("Товар с идентификатором " + id + " не найден");

            ShowcaseRepository.TakeOut(product);
            ProductRepository.Remove(id);

            return new Result(true);
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        internal IResult ShowcaseCreateAction()
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
                return new Result(validateResult.Message);
             
            ShowcaseRepository.Add(showcase);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Вызывает сценарий обновления витрины
        /// </summary>
        /// <returns></returns>
        internal IResult ShowcaseUpdateAction()
        {
            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (!int.TryParse(Console.ReadLine(), out int id))
                return new Result("Идентификатор должен быть целым положительным числом");
            
            Showcase showcase = ShowcaseRepository.GetById(id);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Result("Витрина с идентификатором " + id + " не найдена");

            Output.Write("Наименование (" + showcase.Name + "):");
            var name = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(name))
                showcase.Name = name;

            Output.Write("Максимально допустимый объем витрины (" + showcase.MaxCapacity + "):");

            //Если объем задан корректно, то применяем, в противном случае оставляем как было
            if (!int.TryParse(Console.ReadLine(), out int capacityInt))
                return new Result("Объем должен быть целым положительным числом");

            //Чекаем изменение объема в меньшую сторону
            int showcaseFullness = ProductRepository.ProductsCapacity(ShowcaseRepository.GetShowcaseProducts(showcase));

            if (showcaseFullness > capacityInt)
                return new Result("Невозможно установить заданный объем, объем размещенного товара превышеает значение");

            showcase.MaxCapacity = capacityInt;

            var validateResult = showcase.Validate();

            if (!validateResult.IsSuccess)
                return new Result(validateResult.Message);
                
            ShowcaseRepository.Update(showcase);
            
            return new Result(true);
        }

        /// <summary>
        /// Вызывает сценарий удаления витрины
        /// </summary>
        /// <returns></returns>
        internal IResult ShowcaseRemoveAction()
        {
            Console.Clear();
            Output.WriteLine("Удалить витрину", ConsoleColor.Cyan);

            if (ShowcaseRepository.ActivesCount() == 0)
                return new Result("Нет витрин для удаления");

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины для удаления: ", ConsoleColor.Yellow);
            if (!int.TryParse(Console.ReadLine(), out int id) || id < 1)
                return new Result("Идентификатор должен быть ценым положительным числом");

            Showcase showcase = ShowcaseRepository.GetById(id);

            if (showcase == null)
                return new Result("Витрина не найдена");
             
            if (Enumerable.Count(ShowcaseRepository.GetShowcaseProductsIds(showcase)) != 0)
                return new Result("Невозможно удалить витрину, на которой размещены товары");

            ShowcaseRepository.Remove(showcase.Id);
            return new Result(true);
        }

        /// <summary>
        /// Вызывает сценарий размещения товара на витрине
        /// </summary>
        /// <returns></returns>
        internal IResult PlaceProductAction()
        {
            Console.Clear();

            if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
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
            PrintProductsAction();

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

        /// <summary>
        /// Вызывает сценарий отображения всех товаров
        /// </summary>
        /// <returns></returns>
        internal IResult PrintProductsAction()
        {
            Console.Clear();

            if (ProductRepository.Count() == 0)
                return new Result("Нет товаров для отображения");

            Output.WriteLine("Доступные товары", ConsoleColor.Cyan);            
            foreach (Product product in ProductRepository.All())
                Output.WriteLine(product.ToString());

            return new Result(true);
        }

        /// <summary>
        /// Вызывает сценарий отображения всех товаров на витрине
        /// </summary>
        /// <returns></returns>
        internal IResult PrintShowcaseProductsAction()
        {
            if (ShowcaseRepository.ActivesCount() == 0 || ProductRepository.Count() == 0)
                return new Result("Нет товаров и витрин для отображения");

            PrintShowcasesAction(false);

            Output.Write("\r\nВведите Id витрины: ", ConsoleColor.Yellow);

            if (int.TryParse(Console.ReadLine(), out int id) == false)
                return new Result("Нет витрин с указанным идентификатором");

            var showcase = ShowcaseRepository.GetById(id);

            if (showcase == null || showcase.RemovedAt.HasValue)
                return new Result("Нет витрин с указанным идентификатором");

            Output.Write("\r\nТовары на витрине ");
            Output.WriteLine(showcase.Name + ":", ConsoleColor.Cyan);

            var ids = ShowcaseRepository.GetShowcaseProductsIds(showcase);

            if (Enumerable.Count(ids) == 0)
                return new Result("Нет товаров для отображения");

            foreach (int pId in ids)
            {
                var product = ProductRepository.GetById(pId);

                if (product != null)
                    Output.WriteLine(product.ToString());
            }

            return new Result();
        }

        /// <summary>
        /// Вызывает сценарий отображения витрин
        /// </summary>
        /// <returns></returns>
        internal IResult PrintShowcasesAction(bool showOnlyDeleted = false)
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
                return new Result("Нет витрин для отображения");

            return new Result();
        }

        public IResult PrintActiveShowcases() => PrintShowcasesAction(false);
        public IResult PrintShowcasesInTrash() => PrintShowcasesAction(true);

        #endregion

        /// <summary>
        /// Выводит на экран сообщение о результате выполнения действия
        /// </summary>
        /// <param name="result"></param>
        internal void ShowResult(IResult result)
        {
            if (result.Success && string.IsNullOrWhiteSpace(result.Message))
                result.Message = "Выполнено";

            Output.WriteLine(result.Message, result.Success ? ConsoleColor.Green : ConsoleColor.Red);

            Output.WriteLine("Нажмите любую клавишу для продолжения..", ConsoleColor.Yellow);
            Console.ReadKey();
        }
    }
}
