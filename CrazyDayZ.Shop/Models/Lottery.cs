namespace CrazyDayZ.Shop.Models
{
    public class Lottery
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OldPrice { get; set; }
        public int NewPrice { get; set; }
        public string Discount { get; set; }
        public string ImageUrl { get; set; }
        public bool ShowDiscount { get; set; }
        public int Order { get; set; }        
        public List<string> AvailableServers { get; set; } = new List<string>();
        public List<LotteryProduct> LotteryProducts { get; set; } = new List<LotteryProduct>();
        public Category Category { get; set; }
    }
}
