using System.ComponentModel.DataAnnotations;

namespace Shop.WebApi.Model
{
    public class ShowcaseDTO
    {
        public int Id { get; set; }
        [Required]
        [MinLength(5)]
        public string Name { get; set; }
        [Required]
        [Range(1,int.MaxValue)]
        public int MaxCapacity { get; set; }
    }
}
