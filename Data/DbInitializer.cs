using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EXAMEN_PARCIAL.Models;
namespace EXAMEN_PARCIAL.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            // Verificar si ya hay datos
            if (context.Cursos.Any())
            {
                return;   // La base de datos ya tiene datos
            }

            // Crear cursos
            var cursos = new Curso[]
            {
                new Curso { Codigo = "MAT101", Nombre = "Matemáticas Básicas", Creditos = 4, CupoMaximo = 30, HorarioInicio = new TimeSpan(8, 0, 0), HorarioFin = new TimeSpan(10, 0, 0), Activo = true },
                new Curso { Codigo = "FIS101", Nombre = "Física I", Creditos = 5, CupoMaximo = 25, HorarioInicio = new TimeSpan(10, 0, 0), HorarioFin = new TimeSpan(12, 0, 0), Activo = true },
                new Curso { Codigo = "PROG101", Nombre = "Programación I", Creditos = 6, CupoMaximo = 20, HorarioInicio = new TimeSpan(14, 0, 0), HorarioFin = new TimeSpan(16, 0, 0), Activo = true }
            };

            context.Cursos.AddRange(cursos);
            await context.SaveChangesAsync();

            // Crear rol de Coordinador si no existe
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Coordinador" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Crear usuario coordinador
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var coordinador = new IdentityUser
            {
                UserName = "coordinador@universidad.edu",
                Email = "coordinador@universidad.edu",
                EmailConfirmed = true
            };

            string coordinadorPassword = "Coordinador123!";
            var user = await userManager.FindByEmailAsync(coordinador.Email);

            if (user == null)
            {
                var createPowerUser = await userManager.CreateAsync(coordinador, coordinadorPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(coordinador, "Coordinador");
                }
            }
        }
    }
}