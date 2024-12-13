using CrazyDayZ.Shop.Data;

namespace CrazyDayZ.Shop.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string Price { get; set; }

        public string Time { get; set; }

        public string Server { get; set; }

        public string Promocode { get; set; }

        public string UserId { get; set; } 
        public AppIdentityUser User { get; set; }
    }
}
