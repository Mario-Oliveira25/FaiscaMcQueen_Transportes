using Microsoft.AspNetCore.Identity;

namespace FaiscaMcQueen_Transportes.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            var userManager = service.GetService<UserManager<IdentityUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole("Chefe de Equipa"));
            await roleManager.CreateAsync(new IdentityRole("Utilizador"));

            var chefeEmail = "admin@faiscamcqueen.pt";
            var chefeUser = await userManager.FindByEmailAsync(chefeEmail);

            if (chefeUser == null)
            {
                var newChefe = new IdentityUser
                {
                    UserName = chefeEmail,
                    Email = chefeEmail,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(newChefe, "Atec@123");

                await userManager.AddToRoleAsync(newChefe, "Chefe de Equipa");
            }
        }
    }
}
