﻿namespace Shop.Model
{
    class Product: Entity
    {
        public string Name { get; set; }
        public int Capacity { get; set; }

        public override string ToString()
        {
            return $"[{Id}] {Name} (cap: {Capacity})";
        }

        public override IResult Validate()
        {
            IResult result = new Result(true, string.Empty);

            if (string.IsNullOrWhiteSpace(Name))
            {
                result.Success = false;
                result.Message += "Наименование не должно быть пустым\r\n";
            }

            if (Capacity < 1)
            {
                result.Success = false;
                result.Message += "Занимаемый объем должен быть целым положительным числом\r\n";
            }

            return result;
        }
    }
}