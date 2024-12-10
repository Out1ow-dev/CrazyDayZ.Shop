namespace CrazyDayZ.Shop.Models.Config
{
    public class Config
    {
        public string EntryId { get; set; }
        public int DurationMinutes { get; set; }
        public int Public { get; set; }
        public int SpawnOnGround { get; set; }
        public int OnlyOnRespawn { get; set; }
        public int MaxUsages { get; set; }
        public List<Item> Items { get; set; }
        public List<object> Weapons { get; set; } // Если у вас есть оружие
        public List<Car> Cars { get; set; }
        public List<object> Players { get; set; } // Если у вас есть игроки
    }
}
