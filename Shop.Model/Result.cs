namespace Shop.Model
{
    public class Result : IResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public Result(bool success = false) => IsSuccess = success;
        public Result(string message) => Message = message;
    }
}
