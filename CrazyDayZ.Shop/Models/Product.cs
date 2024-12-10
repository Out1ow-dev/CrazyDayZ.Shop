using CrazyDayZ.Shop.Models.Config;

namespace CrazyDayZ.Shop.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OldPrice { get; set; }
        public string NewPrice { get; set; }
        public string Discount { get; set; }
        public string ImageUrl { get; set; }
        public List<string> AvailableServers { get; set; } = new List<string>();
        public Category Category { get; set; }
        public bool ShowDiscount { get; set; }
        public bool IsAviable { get; set; }
        public int Order { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public List<Car> Cars { get; set; } = new List<Car>();
        
    }
}
