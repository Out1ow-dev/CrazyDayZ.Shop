using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using CrazyDayZ.Shop.Models.Config;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;

namespace CrazyDayZ.Shop.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppIdentityUser> _userManager;
        private static readonly Random _random = new Random();


        public List<Product> Products { get; set; }
        public List<Server> Servers { get; set; }
        public List<Category> Categories { get; set; }
        public List<Lottery> Lotteries { get; set; }

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context, UserManager<AppIdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            Products = new List<Product>();
            Servers = new List<Server>();
            Categories = new List<Category>();
            Lotteries = new List<Lottery>();
        }

        public async void OnGet()
        {
            _logger.LogInformation("OnGet called, populating data.");
            
            Servers = ServerInfo.Servers;         
            Products = _context.Products.OrderBy(c => c.Order).ToList();
            Categories = _context.Categories.OrderBy(c => c.Order).ToList();
            Lotteries =  _context.Lotteries.Include(l => l.LotteryProducts).ThenInclude(lp => lp.Product).ToList();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPurchase(int productId, string activation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "Пожалуйста, авторизуйтесь для покупки." });
            }

            var product = await _context.Products
                .Include(p => p.Items)
                .ThenInclude(i => i.Attachments)
                .Include(p => p.Cars)
                .ThenInclude(c => c.Attachments)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new JsonResult(new { success = false, message = "Товар не найден." });
            }

            if (!product.AvailableServers.Contains(activation))
            {
                return new JsonResult(new { success = false, message = "Товар недоступен на выбранном сервере." });
            }

            decimal userBalance = user.Balance;
            decimal productPrice = decimal.Parse(product.NewPrice);

            if (userBalance < productPrice)
            {
                return new JsonResult(new { success = false, message = "Недостаточно средств для покупки." });
            }

            string entryId = GenerateRandomEntryId(10);

            var config = new
            {
                entry_id = entryId,
                duration_minutes = -1, 
                @public = 1,
                spawn_on_ground = 1,
                only_on_respawn = 0,
                max_usages = 1, 
                items = product.Items.Select(i => new
                {
                    class_name = i.ClassName, 
                    amount = i.Amount, 
                    attachments = i.Attachments.Select(a => new
                    {
                        attachments = new List<object>(),
                        class_name = a.ClassName, 
                        amount = a.Amount 
                    }).ToList()
                }).ToList(),
                weapons = new List<object>(), 
                cars = product.Cars.Select(c => new
                {                    
                    attachments = c.Attachments.Select(a => new
                    {
                        attachments = new List<object>(),
                        class_name = a.ClassName, 
                        amount = a.Amount
                    }).ToList(),
                    class_name = c.ClassName
                }).ToList(),
                players = new List<object>() 
            };

            string jsonConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
            string fileName = $"{entryId}.json";
            System.IO.File.WriteAllText(Path.Combine(@"C:\Users\User\Desktop\Новая папка (14)", fileName), jsonConfig); // Укажите путь для сохранения файла

            user.Balance -= productPrice;


            user.Purchase.Add(new Purchase 
            { 
                ProductName = product.Name,
                Price = productPrice,
                Time = DateTime.Now.ToString("f"),
                Server = activation,
                Promocode = $"!promo {entryId}"
            });

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Товар успешно куплен!", entryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostPurchaseRoulette(int lotteryId, string activation)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "Пожалуйста, авторизуйтесь для получения выигрыша." });
            }

            var lottery = await _context.Lotteries
                .Include(l => l.LotteryProducts)
                .ThenInclude(lp => lp.Product)
                .FirstOrDefaultAsync(l => l.Id == lotteryId);

            if (lottery == null)
            {
                return new JsonResult(new { success = false, message = "Рулетка не найдена." });
            }

            var rouletteCost = lottery.NewPrice; // Стоимость рулетки
            if (user.Balance < rouletteCost)
            {
                return new JsonResult(new { success = false, message = "Недостаточно средств для участия в рулетке." });
            }

            user.Balance -= rouletteCost;

            // Логика для выдачи выигрыша
            var winningProduct = lottery.LotteryProducts
                .OrderBy(lp => _random.Next()).First().Product; // Случайный выигрышный товар

            user.Purchase.Add(new Purchase
            {
                ProductName = winningProduct.Name,
                Price = 0, // Цена может быть 0, если это выигрыш
                Time = DateTime.Now.ToString("f"),
                Server = activation,
                Promocode = $"!promo {GenerateRandomEntryId(10)}" // Генерация промокода для выигрыша
            });

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Выигрыш: " + winningProduct.Name });
        }

        private string GenerateRandomEntryId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
