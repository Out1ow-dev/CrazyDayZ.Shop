namespace CrazyDayZ.Shop.Models.Config
{
    public class Car
    {
        public int Id { get; set; }
        public string ClassName { get; set; }
        public List<CarAttachment> Attachments { get; set; } = new List<CarAttachment>();
        public Product Product { get; set; }  // Связь с продуктом
    }
}
