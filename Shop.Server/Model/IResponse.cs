namespace Shop.Server.Model
{
    public interface IResponse
    {
        int StatusCode { get; set; }
        string ToJson();
    }
}