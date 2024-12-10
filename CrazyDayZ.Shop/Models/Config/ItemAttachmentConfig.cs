namespace CrazyDayZ.Shop.Models.Config
{
    public class ItemAttachmentConfig
    {
        public List<object> Attachments { get; set; } // Если у вас есть вложенные аттачменты
        public string ClassName { get; set; }
        public int Amount { get; set; }
    }
}
