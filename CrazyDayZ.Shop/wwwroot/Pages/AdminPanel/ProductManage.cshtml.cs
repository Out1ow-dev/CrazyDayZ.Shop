using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Models;
using CrazyDayZ.Shop.Models.Config;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CrazyDayZ.Shop.Pages.AdminPanel
{
    public class ProductManageModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProductManageModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Product Product { get; set; }

        [BindProperty]
        public Lottery Lottery { get; set; } // ����� ������ ��� ������ � ���������

        [BindProperty]
        public List<Category> Categories { get; set; }

        [BindProperty]
        public List<ServerData> Servers { get; set; }

        public List<Product> Products { get; set; }
        public List<Lottery> Lotteries { get; set; } // ����� ������ �������

        public async Task OnGetAsync()
        {
            Categories = await _context.Categories.OrderBy(c => c.Order).ToListAsync();
            Servers = await _context.Servers.ToListAsync();
            Products = await _context.Products.Include(p => p.Category).OrderBy(c => c.Order).ToListAsync();
            Lotteries = await _context.Lotteries.OrderBy(l => l.Order).ToListAsync(); // �������� �������
        }

        public async Task<IActionResult> OnPostSaveProductAsync()
        {
            if (!HttpContext.Request.HasFormContentType)
            {
                return BadRequest("Invalid request format.");
            }

            var form = HttpContext.Request.Form;

            // �������������� ����� � ��������������� ���� � ��������� �� ������
            if (!int.TryParse(form["Product.Id"], out int productId))
            {
                productId = 0;
            }

            Product product = new Product
            {
                Id = productId,
                Name = form["Product.Name"],
                Description = form["Product.Description"],
                OldPrice = form["Product.OldPrice"],
                NewPrice = form["Product.NewPrice"],
                Discount = form["Product.Discount"],
                ImageUrl = form["Product.ImageUrl"],
                ShowDiscount = bool.TryParse(form["Product.ShowDiscount"], out bool showDiscount) && showDiscount,
                AvailableServers = form["AvailableServers"].ToList(),
                Category = new Category { Id = int.Parse(form["Product.Category.Id"]) },
                IsAviable = true, // ������������� �� ���������
                Items = new List<Item>(),
                Cars = new List<Car>()
            };

            // ��������� ���������
            for (int i = 0; i < form.Keys.Count; i++)
            {
                if (form[$"Items[{i}].ClassName"].Count > 0)
                {
                    var item = new Item
                    {
                        ClassName = form[$"Items[{i}].ClassName"],
                        Amount = int.TryParse(form[$"Items[{i}].Amount"], out int amount) ? amount : 1
                    };

                    // ��������� ����������� ��� ��������
                    for (int j = 0; j < form.Keys.Count; j++)
                    {
                        if (form[$"Items[{i}].Attachments[{j}].ClassName"].Count > 0)
                        {
                            var attachment = new ItemAttachment
                            {
                                ClassName = form[$"Items[{i}].Attachments[{j}].ClassName"],
                                Amount = int.TryParse(form[$"Items[{i}].Attachments[{j}].Amount"], out int attAmount) ? attAmount : 1
                            };
                            attachment.Item = item; // ������������� ����� � ���������
                            item.Attachments.Add(attachment);
                        }
                    }

                    product.Items.Add(item);
                }
            }

            // ��������� �����������
            for (int i = 0; i < form.Keys.Count; i++)
            {
                if (form[$"Cars[{i}].ClassName"].Count > 0)
                {
                    var car = new Car
                    {
                        ClassName = form[$"Cars[{i}].ClassName"],
                        Attachments = new List<CarAttachment>()
                    };

                    // ��������� ����������� ��� ����������
                    for (int j = 0; j < form.Keys.Count; j++)
                    {
                        if (form[$"Cars[{i}].Attachments[{j}].ClassName"].Count > 0)
                        {
                            var attachment = new CarAttachment
                            {
                                ClassName = form[$"Cars[{i}].Attachments[{j}].ClassName"],
                                Amount = int.TryParse(form[$"Cars[{i}].Attachments[{j}].Amount"], out int carAttAmount) ? carAttAmount : 1
                            };
                            attachment.Car = car; // ������������� ����� � �����������
                            car.Attachments.Add(attachment);
                        }
                    }

                    product.Cars.Add(car);
                }
            }

            // ���������� ��� ���������� ��������
            if (product.Id == 0)
            {
                var maxOrder = await _context.Products.MaxAsync(c => (int?)c.Order) ?? 0;
                product.Order = maxOrder + 1;
                product.Category = await _context.Categories.FindAsync(product.Category.Id);
                _context.Products.Add(product);
            }
            else
            {
                var existingProduct = await _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Items)
                        .ThenInclude(i => i.Attachments)
                    .Include(p => p.Cars)
                        .ThenInclude(c => c.Attachments)
                    .FirstOrDefaultAsync(p => p.Id == product.Id);

                if (existingProduct != null)
                {
                    existingProduct.Name = product.Name;
                    existingProduct.Description = product.Description;
                    existingProduct.OldPrice = product.OldPrice;
                    existingProduct.NewPrice = product.NewPrice;
                    existingProduct.Discount = product.Discount;
                    existingProduct.ImageUrl = product.ImageUrl;
                    existingProduct.ShowDiscount = product.ShowDiscount;
                    existingProduct.Category = await _context.Categories.FindAsync(product.Category.Id);
                    existingProduct.AvailableServers = product.AvailableServers;

                    // ���������� ���������
                    existingProduct.Items.Clear();
                    foreach (var item in product.Items)
                    {
                        existingProduct.Items.Add(item);
                    }

                    // ���������� �����������
                    existingProduct.Cars.Clear();
                    foreach (var car in product.Cars)
                    {
                        existingProduct.Cars.Add(car);
                    }
                }
            }

            // ���������� ��������� � ���� ������
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSaveLotteryAsync()
        {
            if (!HttpContext.Request.HasFormContentType)
            {
                return BadRequest("Invalid request format.");
            }

            var form = HttpContext.Request.Form;

            // ������� ID �������
            if (!int.TryParse(form["Lottery.Id"], out int lotteryId))
            {
                lotteryId = 0;
            }

            // �������� ��������� �� ID
            var categoryId = int.Parse(form["Lottery.Category.Id"]);
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                ModelState.AddModelError("", $"Category with ID {categoryId} not found.");
                return Page();
            }

            Lottery lottery = new Lottery
            {
                Id = lotteryId,
                Name = form["Lottery.Name"],
                OldPrice = int.TryParse(form["Lottery.OldPrice"], out var oldPrice) ? oldPrice : 0,
                NewPrice = int.TryParse(form["Lottery.NewPrice"], out var newPrice) ? newPrice : 0,
                Discount = form["Lottery.Discount"],
                ImageUrl = form["Lottery.ImageUrl"], // ��������� URL ��������
                ShowDiscount = form.ContainsKey("Lottery.ShowDiscount"), // �������� ����������� ������
                AvailableServers = form["AvailableServers"].ToList(), // ��������� ��������� ��������
                LotteryProducts = new List<LotteryProduct>(),
                Category = category, // ������������� ������ �� ������������ ���������
                Order = lotteryId == 0 ? (await _context.Lotteries.MaxAsync(l => (int?)l.Order) ?? 0) + 1 : 0
            };

            // ��������� ��������� � �������
            int totalChance = 0;
            int productIndex = 0; // ������ ��� LotteryProducts
            while (form.ContainsKey($"LotteryProducts[{productIndex}].ProductId"))
            {
                var productIdKey = $"LotteryProducts[{productIndex}].ProductId";
                var chanceKey = $"LotteryProducts[{productIndex}].Chance";

                // �������� ������� ProductId � �����
                if (form[productIdKey].Count > 0 && !string.IsNullOrEmpty(form[productIdKey]))
                {
                    var productId = int.Parse(form[productIdKey]);
                    var product = await _context.Products.FindAsync(productId);

                    if (product != null)
                    {
                        // ������� �������� �����
                        if (int.TryParse(form[chanceKey], out int chance) && chance > 0)
                        {
                            var lotteryProductItem = new LotteryProduct
                            {
                                Product = product,
                                Chance = chance
                            };

                            totalChance += lotteryProductItem.Chance;
                            lottery.LotteryProducts.Add(lotteryProductItem);
                        }
                        else
                        {
                            // ��������� ������������� �������� �����
                            ModelState.AddModelError("", $"Chance value for product ID {productId} is invalid or not specified. It must be greater than 0.");
                        }
                    }
                    else
                    {
                        // ��������� ������, ����� ������� �� ������
                        ModelState.AddModelError("", $"Product with ID {productId} not found.");
                    }
                }
                productIndex++;
            }

            // �������� �� ���������� 100%
            if (totalChance > 100)
            {
                ModelState.AddModelError("", "��������� ������� ������ �� ����� ��������� 100%.");
                return Page();
            }

            // ���������� ��� ���������� �������
            if (lottery.Id == 0)
            {
                _context.Lotteries.Add(lottery);
            }
            else
            {
                var existingLottery = await _context.Lotteries
                    .Include(l => l.LotteryProducts)
                    .FirstOrDefaultAsync(l => l.Id == lottery.Id);

                if (existingLottery != null)
                {
                    existingLottery.Name = lottery.Name;
                    existingLottery.OldPrice = lottery.OldPrice;
                    existingLottery.NewPrice = lottery.NewPrice;
                    existingLottery.Discount = lottery.Discount;
                    existingLottery.ImageUrl = lottery.ImageUrl; // ���������� URL ��������
                    existingLottery.ShowDiscount = lottery.ShowDiscount; // ���������� ����������� ������
                    existingLottery.AvailableServers = lottery.AvailableServers; // ���������� ��������� ��������

                    // ���������� ��������� � �������
                    existingLottery.LotteryProducts.Clear();
                    foreach (var lotteryItem in lottery.LotteryProducts)
                    {
                        existingLottery.LotteryProducts.Add(lotteryItem);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateLotteryOrder(string[] order)
        {
            for (int i = 0; i < order.Length; i++)
            {
                var lotteryId = int.Parse(order[i]);
                var lottery = await _context.Lotteries.FindAsync(lotteryId);
                if (lottery != null)
                {
                    lottery.Order = i + 1; // ������������� ����� �������
                }
            }

            await _context.SaveChangesAsync();
            var updatedProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            var updatedLotteries = await _context.Lotteries.OrderBy(l => l.Order).ToListAsync();

            return Partial("_CombinedTable", new CombinedTableViewModel
            {
                Products = updatedProducts,
                Lotteries = updatedLotteries
            });
        }

        public async Task<IActionResult> OnGetGetLottery(int id)
        {
            var lottery = await _context.Lotteries
                .Include(l => l.Category)
                .Include(l => l.LotteryProducts) // �������� ��������� ��������
                    .ThenInclude(lp => lp.Product) // �������� ��� �������
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lottery == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                id = lottery.Id,
                name = lottery.Name,
                oldPrice = lottery.OldPrice,
                newPrice = lottery.NewPrice,
                discount = lottery.Discount,
                imageUrl = lottery.ImageUrl,
                availableServers = lottery.AvailableServers,
                category = new { Id = lottery.Category.Id },
                showDiscount = lottery.ShowDiscount,
                lotteryProducts = lottery.LotteryProducts.Select(lp => new // ��������� ���������� � ���������
                {
                    productId = lp.Product.Id,
                    productName = lp.Product.Name,
                    chance = lp.Chance
                }).ToList()
            });
        }

        public async Task<IActionResult> OnGetGetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Items)
                    .ThenInclude(i => i.Attachments)
                .Include(p => p.Cars)
                    .ThenInclude(c => c.Attachments)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return new JsonResult(new
            {
                id = product.Id,
                name = product.Name,
                description = product.Description,
                oldPrice = product.OldPrice,
                newPrice = product.NewPrice,
                discount = product.Discount,
                imageUrl = product.ImageUrl,
                availableServers = product.AvailableServers,
                category = new { Id = product.Category.Id },
                showDiscount = product.ShowDiscount,
                items = product.Items.Select(i => new
                {
                    className = i.ClassName,
                    amount = i.Amount,
                    attachments = i.Attachments.Select(a => new
                    {
                        className = a.ClassName,
                        amount = a.Amount
                    }).ToList()
                }).ToList(),
                cars = product.Cars.Select(c => new
                {
                    className = c.ClassName,
                    attachments = c.Attachments.Select(a => new
                    {
                        className = a.ClassName,
                        amount = a.Amount
                    }).ToList()
                }).ToList()
            });
        }

        public async Task<IActionResult> OnPostDeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            var updatedProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            var updatedLotteries = await _context.Lotteries.OrderBy(l => l.Order).ToListAsync();      
            return Partial("_CombinedTable", new CombinedTableViewModel
            {
                Products = updatedProducts,
                Lotteries = updatedLotteries
            });
        }

        public async Task<IActionResult> OnPostDeleteLottery(int id)
        {
            var lottery = await _context.Lotteries.FindAsync(id);
            if (lottery == null)
            {
                return NotFound();
            }

            _context.Lotteries.Remove(lottery);
            await _context.SaveChangesAsync();
            
            var updatedProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            var updatedLotteries = await _context.Lotteries.OrderBy(l => l.Order).ToListAsync();

            return Partial("_CombinedTable", new CombinedTableViewModel
            {
                Products = updatedProducts,
                Lotteries = updatedLotteries
            });
        }

        public async Task<IActionResult> OnPostUpdateOrder(string[] order)
        {
            // ��������� ������� ��� ��������� � �������
            var productIds = new List<int>();
            var lotteryIds = new List<int>();

            foreach (var id in order)
            {
                if (int.TryParse(id, out var parsedId))
                {
                    var product = await _context.Products.FindAsync(parsedId);
                    if (product != null)
                    {
                        productIds.Add(parsedId);
                    }
                    else
                    {
                        var lottery = await _context.Lotteries.FindAsync(parsedId);
                        if (lottery != null)
                        {
                            lotteryIds.Add(parsedId);
                        }
                    }
                }
            }

            // ���������� ������� ��� ���������
            for (int i = 0; i < productIds.Count; i++)
            {
                var product = await _context.Products.FindAsync(productIds[i]);
                if (product != null)
                {
                    product.Order = i + 1; // ������������� ����� �������
                }
            }

            // ���������� ������� ��� �������
            for (int i = 0; i < lotteryIds.Count; i++)
            {
                var lottery = await _context.Lotteries.FindAsync(lotteryIds[i]);
                if (lottery != null)
                {
                    lottery.Order = i + 1; // ������������� ����� �������
                }
            }

            await _context.SaveChangesAsync();

            var updatedProducts = await _context.Products.Include(p => p.Category).ToListAsync();
            var updatedLotteries = await _context.Lotteries.OrderBy(l => l.Order).ToListAsync();

            return Partial("_CombinedTable", new CombinedTableViewModel
            {
                Products = updatedProducts,
                Lotteries = updatedLotteries
            });
        }

    }
}
