using CrazyDayZ.Shop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrazyDayZ.Shop.Pages.AdminPanel
{
    public class UserManageModel : PageModel
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserManageModel(UserManager<AppIdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public List<AppIdentityUser> Users { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();
        }

        public async Task<IActionResult> OnPostUpdateUser(string userId, string email, string SteamId, int Balance)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.Email = email;
                user.SteamId = SteamId;
                user.Balance = Balance; 

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                }
            }
            return RedirectToPage();
        }

    }


}
