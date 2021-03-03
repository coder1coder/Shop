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
    }
}
