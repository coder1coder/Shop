namespace Shop.Model
{
    internal class ValidateResult : IValidateResult
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public ValidateResult(bool isSuccess = false, string message = "" )
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}