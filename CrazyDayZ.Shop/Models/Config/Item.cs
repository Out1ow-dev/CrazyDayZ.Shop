namespace CrazyDayZ.Shop.Models.Config
{
    public class Item
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public List<ItemAttachment> Attachments { get; set; } = new List<ItemAttachment>();
        public int Amount { get; set; }
        public Product Product { get; set; } 
    }
}
