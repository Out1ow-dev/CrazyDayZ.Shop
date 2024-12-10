using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrazyDayZ.Shop.Areas.Identity.Pages.Account.Manage
{
    public class PurchaseHistoryModel : PageModel
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PurchaseHistoryModel(UserManager<AppIdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<Purchase> Purchases { get; set; } = new List<Purchase>();

        public async Task OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                Purchases = await _context.Purchases
                    .Where(p => p.UserId == user.Id)
                    .OrderByDescending(p => p.Id) // Сортировка от большего к меньшему
                    .ToListAsync();
            }
        }
    }
}
