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

            new ProductController().Seed(2);
            new ShowcaseController().Seed(2);

            var absoluteUrl = context.Request.Url.AbsoluteUri.TrimStart('/').Split('/');

            object responseObject = null;

            if (absoluteUrl.Length > 0 && absoluteUrl.Length < 3)
            {
                var controller = absoluteUrl[0];
                var action = (absoluteUrl.Length == 2) ? absoluteUrl[1] : string.Empty;

                switch (controller)
                {
                    case "products":
                        responseObject = new ProductController().GetResponse(action, context);
                        break;

                    case "showcases":
                        responseObject = new ShowcaseController().GetResponse(action, context);
                        break;
                }
            }
            else responseObject = new Response() { Message = "404" };

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
