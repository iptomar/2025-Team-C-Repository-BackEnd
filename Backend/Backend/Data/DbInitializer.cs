using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Garante que a base de dados existe
            context.Database.Migrate();

            // Criação de roles
            string[] roles = { "Admin", "ComissaoHorario", "ComissaoCurso", "Professor" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
            // ----------------------------------

            // Criar um utilizador de teste (Admin)
            var testUserEmail = "admin@email.com";
            var testUser = await userManager.FindByEmailAsync(testUserEmail);

            if (testUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = testUserEmail,
                    Email = testUserEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
            // ----------------------------------
        }
    }
}
