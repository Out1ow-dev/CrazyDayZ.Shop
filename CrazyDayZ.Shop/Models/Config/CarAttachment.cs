namespace CrazyDayZ.Shop.Models.Config
{
    public class CarAttachment
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int Amount { get; set; }
        public Car Car { get; set; }
    }
}
