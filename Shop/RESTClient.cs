using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Shop.ConsoleClient
{
    internal class RESTClient
    {
        private readonly HttpClient _client;

        public RESTClient(string host)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(host)
            };

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("", "login")
            });

            var c = new StringContent("test");

            //_client.PostAsync("")
        }

        public string Get(string controller, string action, string parameters)
        {
            var requestUri = Path.Combine(_client.BaseAddress.Host, "//", controller, "//", action , "?", parameters);
            var response = _client.GetStringAsync(requestUri).Result;
            return response;
        }
    }
}
