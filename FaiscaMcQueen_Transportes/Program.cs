using FaiscaMcQueen_Transportes.Data;
using FaiscaMcQueen_Transportes.Models;
using FaiscaMcQueen_Transportes.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FaiscaMcQueen_Transportes
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<FaiscaMcQueenContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>() // Certifica-te de que as Roles estão ativadas para a Tarefa 1.3
            .AddEntityFrameworkStores<FaiscaMcQueenContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddMemoryCache(options =>
            {
                // Limite de tamanho do cache em memória
                options.CompactionPercentage = 0.25;  // Remove 25% dos dados menos usados quando atinge limite
                options.SizeLimit = 100 * 1024 * 1024; // 100 MB máximo
            });
          

            builder.Services.AddResponseCaching(options =>
            {
                options.MaximumBodySize = 1024 * 1024; // 1 MB de body cacheado
                options.UseCaseSensitivePaths = false;  // /Index e /index são tratadas igual
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCaching();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            using(var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await DbSeeder.SeedRolesAndAdminAsync(services);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Erro ao criar Seed: " + ex.Message);
                }
            }

            app.Run();
        }
    }
}
