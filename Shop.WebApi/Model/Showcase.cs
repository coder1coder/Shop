using System;
using System.Collections.Generic;

namespace Shop.WebApi.Model
{
    public class Showcase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int MaxCapacity { get; set; }
        public int Capacity { get; private set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RemovedAt { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
