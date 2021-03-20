using System;
using System.ComponentModel.DataAnnotations;

namespace Shop.WebApi.Model
{
    public class ProductDTO
    {
        public int Id { get; set; }
        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        [Required]
        [Range(1, 100000)]
        public int Capacity { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [Range(1, int.MaxValue)]
        public int Cost { get; set; }
    }
}
