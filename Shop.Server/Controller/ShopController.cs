using Shop.Model;
using Shop.Server.DAL;
using System;
using System.Collections.Generic;

namespace Shop.Server.Controller
{
    class ShopController
    {
        public ProductController ProductsController { get; set; }
        public ShowcaseController ShowcasesController { get; set; }

        internal IShowcaseRepository ShowcaseRepository { get; set; }

        public ShopController()
        {
            ProductsController = new ProductController();
            ProductsController.Seed(2);

            ShowcasesController = new ShowcaseController();
            ShowcasesController.Seed(2);
        }

        /// <summary>
        /// Выводит на экран сообщение о результате выполнения действия
        /// </summary>
        /// <param name="result"></param>
        void ShowResult(IResult result)
        {
            if (result.IsSuccess && string.IsNullOrWhiteSpace(result.Message))
                result.Message = "Выполнено";

            Output.WriteLine(result.Message, result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red);

            Output.WriteLine("Нажмите любую клавишу для продолжения..", ConsoleColor.Yellow);
            Console.ReadKey();
        }
    }
}
