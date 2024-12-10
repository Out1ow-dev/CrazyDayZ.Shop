using CrazyDayZ.Shop.Models;
using Microsoft.AspNetCore.Identity;

namespace CrazyDayZ.Shop.Data
{
    public class AppIdentityUser : IdentityUser
    {
        public string SteamId { get; set; }
        public decimal Balance { get; set; }
        public List<Purchase> Purchase { get; set; } = new List<Purchase>();

    }
}
