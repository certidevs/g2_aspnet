using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Models;

namespace G2AspNet.Data;

/// <summary>Aplica migraciones y prepara roles y cuentas demo de forma idempotente.</summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Facilita arrancar en clase sin ejecutar comandos SQL manuales.
        context.Database.Migrate();

        var configuration = services.GetRequiredService<IConfiguration>();
        if (!configuration.GetValue("SeedData:Enabled", true))
        {
            return;
        }

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await EnsureRoleAsync(roleManager, RoleNames.User);
        await EnsureRoleAsync(roleManager, RoleNames.Admin);

        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        await EnsureUserAsync(
            userManager,
            username: "admin",
            email: "admin@g2.local",
            displayName: "Administrador",
            password: "Admin123!",
            role: RoleNames.Admin);
        await EnsureUserAsync(
            userManager,
            username: "user",
            email: "user@g2.local",
            displayName: "Usuario de demo",
            password: "User123!",
            role: RoleNames.User);

        if (!context.Academies.Any())
        {
            context.Academies.AddRange(
                new Academy { Name = "Adecco School", Description = "Compañía de RRHH.", Location = "Global" },
                new Academy { Name = "España Educa", Location = "España" },
                new Academy { Name = "Instituto de Camuflaje" } // Para comprobar como se comporta la BBDD con datos en blanco.
            );

            context.SaveChanges();
        }

        if (!context.Classrooms.Any())
        {
            var adeccoSchool = context.Academies.First(a => a.Name == "Adecco School");
            var espanaEduca = context.Academies.First(a => a.Name == "España Educa");
            var institutoCamuflaje = context.Academies.First(a => a.Name == "Instituto de Camuflaje");

            context.Classrooms.AddRange(
                new Classroom { Name = "Primer Año", Capacity = 30, IsActive = true},
                new Classroom { Name = "Segundo Año", Capacity = 25, IsActive = false},
                new Classroom { Name = "Adecco Online", Capacity = 15, MeetingUrl = "inserturl", IsActive = true}
            );

            context.SaveChanges();
        }

        if (!context.Grades.Any())
        {
            context.Grades.AddRange(
                new Grade { EnrollmentId = 1, Value = 8.8, Comments = "Bien Hecho" },
                new Grade { EnrollmentId = 2, Value = 10, Comments = "Excelente" }
            );

            context.SaveChanges();
        }
    }

    private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el rol {roleName}.");
            }
        }
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        string username,
        string email,
        string displayName,
        string password,
        string role)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = username,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true,
                IsActive = true
            };
            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo crear el usuario demo {username}.");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException($"No se pudo asignar el rol {role} a {username}.");
            }
        }
    }
}
