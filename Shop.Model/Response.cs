using Newtonsoft.Json;

namespace Shop.Model
{
    public class Response: IResponse
    {
        public int StatusCode { get; set; }
        public string StatusMessage { get; set; }
        public object Content { get; set; }

        public Response(int statusCode) => StatusCode = statusCode;
        public Response(int statusCode, object content)
        {
            StatusCode = statusCode;
            Content = content;
        }

        public Response(int statusCode, string statusMessage)
        {
            StatusCode = statusCode;
            StatusMessage = statusMessage;
        }

        public string ToJson()
        {
            return StatusCode switch
            {
                200 => JsonConvert.SerializeObject(new { code = StatusCode, response = Content }),
                400 => JsonConvert.SerializeObject(new { code = StatusCode, error = true, message = StatusMessage }),
                404 => JsonConvert.SerializeObject(new { code = StatusCode, error = true, message = StatusMessage }),
                _ => JsonConvert.SerializeObject(new { code = 500, error = true, message = StatusMessage }),
            };
        }
    }
}
