namespace CrazyDayZ.Shop.Models
{
    public class Server
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Connect { get; set; }
        public int OnlinePlayersPercentage { get; set; }
        public int CurrentPlayers { get; set; }
        public int MaxPlayers { get; set; }
    }
}
