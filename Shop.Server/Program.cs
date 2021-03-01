using Shop.Server.Controller;
using System;
using System.IO;
using System.Net;

namespace Shop.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
            listener.Start();

            Console.WriteLine("Ожидание подключений...");
            var context = listener.GetContext();

            var shop = new ShopController();

            var data = context.Request.QueryString;

            var command = context.Request.Url.AbsoluteUri.TrimStart('/').ToLower();

            switch (command)
            {
                case "products":
                    break;
                case "products/show":
                     PrintProductsAction();
                    break;
                case "product/create":
                    shop.ProductCreateAction
                    ShowResult(ProductCreateAction());
                    break;
                case "product/edit":
                    ShowResult(ProductUpdateAction());
                    break;
                case "product/remove":
                    ShowResult(ProductRemoveAction());
                    break;
                case "showcase/show":
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
            }

            var request = context.Request;
            HttpListenerResponse response = context.Response;
            string responseStr = "<html><head><meta charset='utf8'></head><body>Привет мир!</body></html>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseStr);
            response.ContentLength64 = buffer.Length;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
            listener.Stop();
            Console.WriteLine("Обработка подключений завершена");
            Console.Read();
        }
    }
}
