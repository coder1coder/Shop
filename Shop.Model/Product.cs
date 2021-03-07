namespace Shop.Model
{
    public class Product: Entity
    {
        public string Name { get; set; }
        public int Capacity { get; set; }

        public override IValidateResult Validate()
        {
            var result = new ValidateResult(true);

            if (string.IsNullOrWhiteSpace(Name))
            {
                result.IsSuccess = false;
                result.Message += "Наименование не должно быть пустым\r\n";
            }

            if (Capacity < 1)
            {
                result.IsSuccess = false;
                result.Message += "Занимаемый объем должен быть целым положительным числом\r\n";
            }

            return result;
        }
    }
}
