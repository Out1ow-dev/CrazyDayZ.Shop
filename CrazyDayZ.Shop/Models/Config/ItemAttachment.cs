namespace CrazyDayZ.Shop.Models.Config
{
    public class ItemAttachment
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int Amount { get; set; }
        public Item Item { get; set; }
    }
}
