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
        await EnsureRoleAsync(roleManager, RoleNames.Admin);
        await EnsureRoleAsync(roleManager, RoleNames.Teacher);
        await EnsureRoleAsync(roleManager, RoleNames.Student);

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
            role: RoleNames.Teacher);

        if (!context.Academies.Any())
        {
            context.Academies.AddRange(
                new Academy { Name = "Adecco School", Description = "Compañía especializada en recursos humanos y formación profesional.", Location = "Global" },
                new Academy { Name = "España Educa", Description = "Centro dedicado a la formación online y presencial para estudiantes y profesionales.", Location = "España" },
                new Academy { Name = "Instituto de Camuflaje", Description = "Academia experimental utilizada para pruebas y validaciones de la aplicación.", Location = "Madrid" },
                new Academy { Name = "Tech Masters", Description = "Escuela enfocada en programación, desarrollo web y nuevas tecnologías.", Location = "Barcelona" },
                new Academy { Name = "Business Academy", Description = "Formación especializada en gestión empresarial, finanzas y liderazgo.", Location = "Valencia" },
                new Academy { Name = "Digital Skills Hub", Description = "Centro de capacitación en marketing digital, SEO y analítica web.", Location = "Sevilla" },
                new Academy { Name = "Creative Design Lab", Description = "Academia centrada en diseño gráfico, UX/UI e ilustración digital.", Location = "Bilbao" },
                new Academy { Name = "Future Coders", Description = "Programas formativos para desarrolladores junior y perfiles tecnológicos.", Location = "Málaga" },
                new Academy { Name = "Global Languages", Description = "Escuela de idiomas con cursos de inglés, francés y alemán.", Location = "Madrid" },
                new Academy { Name = "Health Learning Center", Description = "Academia especializada en formación sanitaria y bienestar personal.", Location = "Zaragoza" },
                new Academy { Name = "Industrial Training", Description = "Centro de formación técnica para sectores industriales y productivos.", Location = "Vigo" },
                new Academy { Name = "Data Science Academy", Description = "Cursos avanzados de análisis de datos, IA y aprendizaje automático.", Location = "Barcelona" },
                new Academy { Name = "Green Energy School", Description = "Formación en energías renovables, sostenibilidad y medio ambiente.", Location = "Pamplona" },
                new Academy { Name = "Hospitality Experts", Description = "Academia orientada al turismo, hostelería y atención al cliente.", Location = "Palma de Mallorca" },
                new Academy { Name = "Cyber Security Hub", Description = "Centro especializado en ciberseguridad, redes y protección de sistemas.", Location = "Alicante" },
                new Academy { Name = "Innovation Academy", Description = "Escuela de innovación, emprendimiento y gestión de proyectos digitales.", Location = "Granada" }
            );

            context.SaveChanges();
        }

        if (!context.Classrooms.Any())
        {
            var adeccoSchool = context.Academies.First(a => a.Name == "Adecco School");
            var espanaEduca = context.Academies.First(a => a.Name == "España Educa");
            var institutoCamuflaje = context.Academies.First(a => a.Name == "Instituto de Camuflaje");
            var techMasters = context.Academies.First(a => a.Name == "Tech Masters");
            var businessAcademy = context.Academies.First(a => a.Name == "Business Academy");
            var digitalSkillsHub = context.Academies.First(a => a.Name == "Digital Skills Hub");
            var creativeDesignLab = context.Academies.First(a => a.Name == "Creative Design Lab");
            var futureCoders = context.Academies.First(a => a.Name == "Future Coders");
            var globalLanguages = context.Academies.First(a => a.Name == "Global Languages");
            var dataScienceAcademy = context.Academies.First(a => a.Name == "Data Science Academy");
            var cyberSecurityHub = context.Academies.First(a => a.Name == "Cyber Security Hub");

            context.Classrooms.AddRange(
                new Classroom { Name = "Primer Año", Capacity = 30, IsActive = true, AcademyId = adeccoSchool.Id },
                new Classroom { Name = "Segundo Año", Capacity = 25, IsActive = false, AcademyId = adeccoSchool.Id },
                new Classroom { Name = "Adecco Online", Capacity = 15, MeetingUrl = "https://meet.adecco.com/aula1", IsActive = true, AcademyId = adeccoSchool.Id },
                new Classroom { Name = "Sala Norte", Capacity = 28, IsActive = true, AcademyId = adeccoSchool.Id },

                new Classroom { Name = "Historia Moderna", Capacity = 40, IsActive = true, AcademyId = espanaEduca.Id },
                new Classroom { Name = "Matemáticas A", Capacity = 32, IsActive = true, AcademyId = espanaEduca.Id },
                new Classroom { Name = "Lengua Castellana", Capacity = 30, IsActive = true, AcademyId = espanaEduca.Id },
                new Classroom { Name = "Aula Virtual ESO", Capacity = 50, MeetingUrl = "https://campus.espanaeduca.es/eso", IsActive = true, AcademyId = espanaEduca.Id },

                new Classroom { Name = "Sigilo I", Capacity = 15, IsActive = true, AcademyId = institutoCamuflaje.Id },
                new Classroom { Name = "Camuflaje Urbano", Capacity = 18, IsActive = true, AcademyId = institutoCamuflaje.Id },
                new Classroom { Name = "Centro de Mando", Capacity = 25, IsActive = true, AcademyId = institutoCamuflaje.Id },
                new Classroom { Name = "Online Encubierto", Capacity = 30, MeetingUrl = "https://camuflaje.edu/online", IsActive = true, AcademyId = institutoCamuflaje.Id },

                new Classroom { Name = "Programación Web", Capacity = 24, IsActive = true, AcademyId = techMasters.Id },
                new Classroom { Name = "Angular Lab", Capacity = 20, IsActive = true, AcademyId = techMasters.Id },
                new Classroom { Name = "NET Advanced", Capacity = 18, IsActive = true, AcademyId = techMasters.Id },
                new Classroom { Name = "DevOps Online", Capacity = 30, MeetingUrl = "https://techmasters.com/devops", IsActive = true, AcademyId = techMasters.Id },

                new Classroom { Name = "Finanzas 101", Capacity = 25, IsActive = true, AcademyId = businessAcademy.Id },
                new Classroom { Name = "Dirección Estratégica", Capacity = 22, IsActive = true, AcademyId = businessAcademy.Id },
                new Classroom { Name = "Gestión de Equipos", Capacity = 20, IsActive = false, AcademyId = businessAcademy.Id },
                new Classroom { Name = "MBA Virtual", Capacity = 45, MeetingUrl = "https://businessacademy.edu/mba", IsActive = true, AcademyId = businessAcademy.Id },

                new Classroom { Name = "SEO Fundamentals", Capacity = 16, IsActive = true, AcademyId = digitalSkillsHub.Id },
                new Classroom { Name = "Google Ads Pro", Capacity = 18, IsActive = true, AcademyId = digitalSkillsHub.Id },
                new Classroom { Name = "Social Media Lab", Capacity = 20, IsActive = true, AcademyId = digitalSkillsHub.Id },
                new Classroom { Name = "Marketing Online", Capacity = 35, MeetingUrl = "https://dshub.es/online", IsActive = true, AcademyId = digitalSkillsHub.Id },

                new Classroom { Name = "Diseño UX", Capacity = 15, IsActive = true, AcademyId = creativeDesignLab.Id },
                new Classroom { Name = "Diseño UI", Capacity = 15, IsActive = true, AcademyId = creativeDesignLab.Id },
                new Classroom { Name = "Figma Studio", Capacity = 20, IsActive = true, AcademyId = creativeDesignLab.Id },
                new Classroom { Name = "Arte Digital", Capacity = 25, IsActive = false, AcademyId = creativeDesignLab.Id },

                new Classroom { Name = "Coding Basics", Capacity = 30, IsActive = true, AcademyId = futureCoders.Id },
                new Classroom { Name = "Java Essentials", Capacity = 24, IsActive = true, AcademyId = futureCoders.Id },
                new Classroom { Name = "Python Lab", Capacity = 20, IsActive = true, AcademyId = futureCoders.Id },
                new Classroom { Name = "Bootcamp Remote", Capacity = 40, MeetingUrl = "https://futurecoders.dev/remote", IsActive = true, AcademyId = futureCoders.Id },

                new Classroom { Name = "English A1", Capacity = 22, IsActive = true, AcademyId = globalLanguages.Id },
                new Classroom { Name = "English C1", Capacity = 18, IsActive = true, AcademyId = globalLanguages.Id },
                new Classroom { Name = "French B1", Capacity = 20, IsActive = true, AcademyId = globalLanguages.Id },
                new Classroom { Name = "German Online", Capacity = 30, MeetingUrl = "https://languages.global/german", IsActive = true, AcademyId = globalLanguages.Id },

                new Classroom { Name = "Machine Learning", Capacity = 20, IsActive = true, AcademyId = dataScienceAcademy.Id },
                new Classroom { Name = "Data Analytics", Capacity = 25, IsActive = true, AcademyId = dataScienceAcademy.Id },
                new Classroom { Name = "Power BI Lab", Capacity = 18, IsActive = false, AcademyId = dataScienceAcademy.Id },
                new Classroom { Name = "Big Data Online", Capacity = 45, MeetingUrl = "https://datascience.academy/bigdata", IsActive = true, AcademyId = dataScienceAcademy.Id },

                new Classroom { Name = "Ethical Hacking", Capacity = 16, IsActive = true, AcademyId = cyberSecurityHub.Id },
                new Classroom { Name = "Network Security", Capacity = 22, IsActive = true, AcademyId = cyberSecurityHub.Id },
                new Classroom { Name = "SOC Operations", Capacity = 18, IsActive = true, AcademyId = cyberSecurityHub.Id },
                new Classroom { Name = "Cyber Range", Capacity = 30, MeetingUrl = "https://cyberhub.org/range", IsActive = true, AcademyId = cyberSecurityHub.Id }
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
