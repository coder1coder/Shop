using Newtonsoft.Json;
using Shop.Model;
using Shop.Server.Controller;
using System;
using System.Net;
using System.Text;

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

            var absoluteUrl = context.Request.Url.AbsoluteUri.TrimStart('/').Split('/');

            object responseObject = null;

            if (absoluteUrl.Length > 0 && absoluteUrl.Length < 3)
            {
                var controller = absoluteUrl[0];
                var action = (absoluteUrl.Length == 2) ? absoluteUrl[1] : string.Empty;

                switch (controller)
                {
                    case "products":
                        responseObject = shop.ProductsController.RouteToAction(action, context.Request);
                        break;

                    case "showcases":
                        responseObject = shop.ShowcasesController.RouteToAction(action, context.Request);
                        break;
                }
            }
            else responseObject = new Result() { Message = "404" };

            var response = JsonConvert.SerializeObject(responseObject);
            var buffer = Encoding.UTF8.GetBytes(response);

            context.Response.ContentType = "application/json";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
            listener.Stop();
            Console.ReadKey();
        }
    }
}
