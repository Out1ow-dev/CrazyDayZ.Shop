namespace CrazyDayZ.Shop.Models.Config
{
    public class Attachment
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public int Amount { get; set; }
        public int CarId { get; set; }  // Связь с автомобилем
        public Car Car { get; set; }
        public int ItemId { get; set; } // Связь с предметом
        public Item Item { get; set; }
    }
}
