using CrazyDayZ.Shop.Data;
using CrazyDayZ.Shop.Extensions;
using CrazyDayZ.Shop.Models;
using CrazyDayZ.Shop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;


namespace CrazyDayZ.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<AppIdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddRazorPages()
                .AddRazorRuntimeCompilation();

            builder.Services.Configure<AuthMessageSenderOptions>(
                builder.Configuration.GetSection("AuthMessageSenderOptions"));

            builder.Services.AddTransient<IEmailSender, EmailSender>();

            builder.Services.AddScoped<MonitoringService>();
            builder.Services.AddHostedService<MonitoringService>(); 

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
                
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.ApplyMigrations();
            app.Run();
        }
    }
}
