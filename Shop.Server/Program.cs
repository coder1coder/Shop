using Shop.Model;
using Shop.RESTApi.Controller;
using System;
using System.Net;
using System.Text;

namespace Shop.RESTApi
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8888/");
            listener.Start();

            new ProductController().Seed(2);
            new ShowcaseController().Seed(2);

            Console.WriteLine("Ожидание подключений...");
            var context = listener.GetContext();

            var absoluteUrl = context.Request.Url.AbsoluteUri.TrimStart('/').Split('/');

            IResponse response = new Response(404);

            if (absoluteUrl.Length > 0 && absoluteUrl.Length < 3)
            {
                var controller = absoluteUrl[0];
                var action = (absoluteUrl.Length == 2) ? absoluteUrl[1] : string.Empty;

                switch (controller)
                {
                    case "products":
                        response = new ProductController().GetResponse(action, context);
                        break;

                    case "showcases":
                        response = new ShowcaseController().GetResponse(action, context);
                        break;
                }
            }

            var buffer = Encoding.UTF8.GetBytes(response.ToJson());

            context.Response.ContentType = "application/json";
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
            listener.Stop();
            Console.ReadKey();
        }
    }
}
