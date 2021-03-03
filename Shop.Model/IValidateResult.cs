namespace Shop.Model
{
    public interface IValidateResult
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
    }
}