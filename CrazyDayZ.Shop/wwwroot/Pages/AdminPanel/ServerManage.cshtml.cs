using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrazyDayZ.Shop.Pages.AdminPanel
{
    public class ServerManageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ServerManageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ServerData ServerData { get; set; }

        public List<ServerData> Servers { get; set; }

        public async Task OnGetAsync()
        {
            Servers = await _context.Servers.ToListAsync(); 
        }

        public async Task<IActionResult> OnPostSaveServerAsync()
        {

            if (ServerData.Id == 0) 
            {
                _context.Servers.Add(ServerData);
            }
            else 
            {
                var existingServer = await _context.Servers.FindAsync(ServerData.Id);
                if (existingServer != null)
                {
                    existingServer.Name = ServerData.Name;
                    existingServer.IP = ServerData.IP;
                    existingServer.Port = ServerData.Port;
                    existingServer.Password = ServerData.Password;
                    existingServer.MaxSlots = ServerData.MaxSlots;
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToPage(); // Перенаправление на ту же страницу
        }

        public async Task<IActionResult> OnGetGetServerAsync(int id)
        {
            ServerData = await _context.Servers.FindAsync(id);
            if (ServerData == null)
            {
                return NotFound();
            }
            return new JsonResult(ServerData);
        }


        public async Task<IActionResult> OnPostDeleteServerAsync(int id)
        {
            var server = await _context.Servers.FindAsync(id);
            if (server == null)
            {
                return NotFound(); // Возвращаем ошибку, если сервер не найден
            }

            _context.Servers.Remove(server);
            await _context.SaveChangesAsync();

            // Обновляем таблицу серверов
            var servers = await _context.Servers.ToListAsync();
            return Partial("_ServersTable", servers);
        }


    }

}
