using Abp.Domain.Entities;

namespace Abp.Dapper.Tests.Entities
{
    public class Product : Entity
    {
        public Product(string name, string color, int size)
        {
            Name = name;
            Color = color;
            Size = size;
        }

        public string Name { get; set; }

        public string Color { get; set; }

        public int Size { get; set; }
    }
}
