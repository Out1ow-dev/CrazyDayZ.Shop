namespace CrazyDayZ.Shop.Models
{
    public class ServerData
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string IP {  get; set; }

        public int Port { get; set; }

        public string Password {  get; set; }
        
        public string MaxSlots { get; set; }
    }
}
