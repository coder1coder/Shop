using System.Collections.Generic;

namespace Shop.WebApi.Model
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int Cost { get; set; }

        public ICollection<Showcase> Showcases { get; set; }

        public Product()
        {

        }

        public static Product FromDTO(ProductDTO productDTO)
        {
            return new Product()
            {
                Id = productDTO.Id,
                Name = productDTO.Name,
                Capacity = productDTO.Capacity,
                Cost = productDTO.Cost
            };
        }
    }
}
