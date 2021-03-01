namespace Shop.Model
{
    public interface IResult
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }

    }
}