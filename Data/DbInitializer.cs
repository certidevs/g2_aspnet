using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Models;

namespace G2AspNet.Data;

/// <summary>Recrea la base de datos desde las entidades y prepara roles y cuentas demo.</summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Proyecto de clase sin migraciones: la base se borra y se vuelve a crear
        // desde las entidades en cada arranque (como create-drop con H2 en Spring).
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

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
        await EnsureUserAsync(
            userManager,
            username: "student",
            email: "student@g2.local",
            displayName: "Estudiante de demo",
            password: "Student123!",
            role: RoleNames.Student);
        // Segundo profesor y dos alumnos más, para repartir cursos, matrículas, notas y asistencias.
        await EnsureUserAsync(
            userManager,
            username: "teacher",
            email: "teacher@g2.local",
            displayName: "Marta Ruiz",
            password: "Teacher123!",
            role: RoleNames.Teacher);
        await EnsureUserAsync(
            userManager,
            username: "student2",
            email: "student2@g2.local",
            displayName: "Carlos Gómez",
            password: "Student123!",
            role: RoleNames.Student);
        await EnsureUserAsync(
            userManager,
            username: "student3",
            email: "student3@g2.local",
            displayName: "Lucía Fernández",
            password: "Student123!",
            role: RoleNames.Student);

        // Cuentas demo ya creadas: "user" y "teacher" son profesores; "student", "student2" y "student3", alumnos.
        var teacher1 = context.Users.First(u => u.UserName == "user");
        var teacher2 = context.Users.First(u => u.UserName == "teacher");
        var student1 = context.Users.First(u => u.UserName == "student");
        var student2 = context.Users.First(u => u.UserName == "student2");
        var student3 = context.Users.First(u => u.UserName == "student3");

        if (!context.Academies.Any())
        {
            context.Academies.AddRange(
                new Academy { Name = "Adecco School", Description = "Compañía especializada en recursos humanos y formación profesional.", Address = "Calle de la Innovación, 12", City = "Madrid", Phone = "+34 912 345 678" },
                new Academy { Name = "España Educa", Description = "Centro dedicado a la formación online y presencial para estudiantes y profesionales.", Address = "Avenida del Conocimiento, 45", City = "Barcelona", Phone = "+34 934 567 890" },
                new Academy { Name = "Instituto de Camuflaje", Description = "Academia experimental utilizada para pruebas y validaciones de la aplicación.", Address = "Calle del Camuflaje, 1", City = "Madrid", Phone = "+34 912 345 678" },
                new Academy { Name = "Tech Masters", Description = "Escuela enfocada en programación, desarrollo web y nuevas tecnologías.", Address = "Avenida de la Tecnología, 100", City = "Barcelona", Phone = "+34 934 567 890" },
                new Academy { Name = "Business Academy", Description = "Formación especializada en gestión empresarial, finanzas y liderazgo.", Address = "Calle del Negocio, 25", City = "Valencia", Phone = "+34 963 123 456" },
                new Academy { Name = "Digital Skills Hub", Description = "Centro de capacitación en marketing digital, SEO y analítica web.", Address = "Avenida de los Datos, 75", City = "Sevilla", Phone = "+34 954 987 654" },
                new Academy { Name = "Creative Design Lab", Description = "Academia centrada en diseño gráfico, UX/UI e ilustración digital.", Address = "Calle del Diseño, 50", City = "Bilbao", Phone = "+34 944 111 222" },
                new Academy { Name = "Future Coders", Description = "Programas formativos para desarrolladores junior y perfiles tecnológicos.", Address = "Avenida de la Tecnología, 150", City = "Málaga", Phone = "+34 952 345 678" },
                new Academy { Name = "Global Languages", Description = "Escuela de idiomas con cursos de inglés, francés y alemán.", Address = "Calle del Idioma, 30", City = "Madrid", Phone = "+34 912 345 678" },
                new Academy { Name = "Health Learning Center", Description = "Academia especializada en formación sanitaria y bienestar personal.", Address = "Avenida de la Salud, 20", City = "Zaragoza", Phone = "+34 976 123 456" },
                new Academy { Name = "Industrial Training", Description = "Centro de formación técnica para sectores industriales y productivos.", Address = "Calle Industrial, 10", City = "Vigo", Phone = "+34 986 789 012" },
                new Academy { Name = "Data Science Academy", Description = "Cursos avanzados de análisis de datos, IA y aprendizaje automático.", Address = "Avenida de los Datos, 80", City = "Barcelona", Phone = "+34 934 567 890" },
                new Academy { Name = "Green Energy School", Description = "Formación en energías renovables, sostenibilidad y medio ambiente.", Address = "Calle de la Energía, 40", City = "Pamplona", Phone = "+34 948 567 890" },
                new Academy { Name = "Hospitality Experts", Description = "Academia orientada al turismo, hostelería y atención al cliente.", Address = "Avenida del Turismo, 60", City = "Palma de Mallorca", Phone = "+34 971 234 567" },
                new Academy { Name = "Cyber Security Hub", Description = "Centro especializado en ciberseguridad, redes y protección de sistemas.", Address = "Calle de la Seguridad, 25", City = "Alicante", Phone = "+34 965 432 109" },
                new Academy { Name = "Innovation Academy", Description = "Escuela de innovación, emprendimiento y gestión de proyectos digitales.", Address = "Avenida de la Innovación, 100", City = "Granada", Phone = "+34 958 765 432" }
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
            var healthLearningCenter = context.Academies.First(a => a.Name == "Health Learning Center");
            var industrialTraining = context.Academies.First(a => a.Name == "Industrial Training");
            var greenEnergySchool = context.Academies.First(a => a.Name == "Green Energy School");
            var hospitalityExperts = context.Academies.First(a => a.Name == "Hospitality Experts");
            var innovationAcademy = context.Academies.First(a => a.Name == "Innovation Academy");

            context.Classrooms.AddRange(
                new Classroom { Name = "Primer Año", Capacity = 30, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = adeccoSchool },
                new Classroom { Name = "Segundo Año", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = false, Academy = adeccoSchool },
                new Classroom { Name = "Adecco Online", Capacity = 15, ClassroomType = ClassroomType.Online, MeetingUrl = "https://meet.adecco.com/aula1", IsActive = true, Academy = adeccoSchool },
                new Classroom { Name = "Sala Norte", Capacity = 28, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = adeccoSchool },

                new Classroom { Name = "Historia Moderna", Capacity = 40, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = espanaEduca },
                new Classroom { Name = "Matemáticas A", Capacity = 32, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = espanaEduca },
                new Classroom { Name = "Lengua Castellana", Capacity = 30, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = espanaEduca },
                new Classroom { Name = "Aula Virtual ESO", Capacity = 50, ClassroomType = ClassroomType.Online, MeetingUrl = "https://campus.espanaeduca.es/eso", IsActive = true, Academy = espanaEduca },

                new Classroom { Name = "Sigilo I", Capacity = 15, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = institutoCamuflaje },
                new Classroom { Name = "Camuflaje Urbano", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = institutoCamuflaje },
                new Classroom { Name = "Centro de Mando", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = institutoCamuflaje },
                new Classroom { Name = "Online Encubierto", Capacity = 30, ClassroomType = ClassroomType.Online, MeetingUrl = "https://camuflaje.edu/online", IsActive = true, Academy = institutoCamuflaje },

                new Classroom { Name = "Programación Web", Capacity = 24, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = techMasters },
                new Classroom { Name = "Angular Lab", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = techMasters },
                new Classroom { Name = "NET Advanced", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = techMasters },
                new Classroom { Name = "DevOps Online", Capacity = 30, ClassroomType = ClassroomType.Online, MeetingUrl = "https://techmasters.com/devops", IsActive = true, Academy = techMasters },

                new Classroom { Name = "Finanzas 101", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = businessAcademy },
                new Classroom { Name = "Dirección Estratégica", Capacity = 22, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = businessAcademy },
                new Classroom { Name = "Gestión de Equipos", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = false, Academy = businessAcademy },
                new Classroom { Name = "MBA Virtual", Capacity = 45, ClassroomType = ClassroomType.Online, MeetingUrl = "https://businessacademy.edu/mba", IsActive = true, Academy = businessAcademy },

                new Classroom { Name = "SEO Fundamentals", Capacity = 16, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = digitalSkillsHub },
                new Classroom { Name = "Google Ads Pro", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = digitalSkillsHub },
                new Classroom { Name = "Social Media Lab", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = digitalSkillsHub },
                new Classroom { Name = "Marketing Online", Capacity = 35, ClassroomType = ClassroomType.Online, MeetingUrl = "https://dshub.es/online", IsActive = true, Academy = digitalSkillsHub },

                new Classroom { Name = "Diseño UX", Capacity = 15, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = creativeDesignLab },
                new Classroom { Name = "Diseño UI", Capacity = 15, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = creativeDesignLab },
                new Classroom { Name = "Figma Studio", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = creativeDesignLab },
                new Classroom { Name = "Arte Digital", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = false, Academy = creativeDesignLab },

                new Classroom { Name = "Coding Basics", Capacity = 30, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = futureCoders },
                new Classroom { Name = "Java Essentials", Capacity = 24, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = futureCoders },
                new Classroom { Name = "Python Lab", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = futureCoders },
                new Classroom { Name = "Bootcamp Remote", Capacity = 40, ClassroomType = ClassroomType.Online, MeetingUrl = "https://futurecoders.dev/remote", IsActive = true, Academy = futureCoders },

                new Classroom { Name = "English A1", Capacity = 22, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = globalLanguages },
                new Classroom { Name = "English C1", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = globalLanguages },
                new Classroom { Name = "French B1", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = globalLanguages },
                new Classroom { Name = "German Online", Capacity = 30, ClassroomType = ClassroomType.Online, MeetingUrl = "https://languages.global/german", IsActive = true, Academy = globalLanguages },

                new Classroom { Name = "Machine Learning", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = dataScienceAcademy },
                new Classroom { Name = "Data Analytics", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = dataScienceAcademy },
                new Classroom { Name = "Power BI Lab", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = false, Academy = dataScienceAcademy },
                new Classroom { Name = "Big Data Online", Capacity = 45, ClassroomType = ClassroomType.Online, MeetingUrl = "https://datascience.academy/bigdata", IsActive = true, Academy = dataScienceAcademy },

                new Classroom { Name = "Ethical Hacking", Capacity = 16, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = cyberSecurityHub },
                new Classroom { Name = "Network Security", Capacity = 22, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = cyberSecurityHub },
                new Classroom { Name = "SOC Operations", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = cyberSecurityHub },
                new Classroom { Name = "Cyber Range", Capacity = 30, ClassroomType = ClassroomType.Online, MeetingUrl = "https://cyberhub.org/range", IsActive = true, Academy = cyberSecurityHub },

                // Un aula para cada academia que aún no tenía ninguna.
                new Classroom { Name = "Sala de Simulación", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = healthLearningCenter },
                new Classroom { Name = "Taller de Mantenimiento", Capacity = 15, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = industrialTraining },
                new Classroom { Name = "Laboratorio Solar", Capacity = 18, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = greenEnergySchool },
                new Classroom { Name = "Aula de Cocina", Capacity = 20, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = hospitalityExperts },
                new Classroom { Name = "Innovation Lab", Capacity = 25, ClassroomType = ClassroomType.Physic, IsActive = true, Academy = innovationAcademy }
            );

            context.SaveChanges();
        }

        if (!context.Courses.Any())
        {
            // Las referencias se declaran en este ámbito porque las variables
            // usadas al crear las aulas pertenecen a un bloque distinto.
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
            var healthLearningCenter = context.Academies.First(a => a.Name == "Health Learning Center");
            var industrialTraining = context.Academies.First(a => a.Name == "Industrial Training");
            var greenEnergySchool = context.Academies.First(a => a.Name == "Green Energy School");
            var hospitalityExperts = context.Academies.First(a => a.Name == "Hospitality Experts");
            var innovationAcademy = context.Academies.First(a => a.Name == "Innovation Academy");

            context.Courses.AddRange(
                new Course { Name = "Primeros pasos en la gestión de personas", Description = "Fundamentos de selección, acogida y desarrollo profesional.", Level = CourseLevel.Beginner, Price = 180, StartDate = new DateOnly(2026, 11, 3), EndDate = new DateOnly(2026, 12, 1), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = adeccoSchool.Id },
                new Course { Name = "Historia de España en movimiento", Description = "Recorrido por los cambios sociales y culturales de la España contemporánea.", Level = CourseLevel.Intermediate, Price = 210, StartDate = new DateOnly(2026, 11, 5), EndDate = new DateOnly(2026, 12, 10), MaxStudents = 30, Status = CourseStatus.Open, AcademyId = espanaEduca.Id },
                new Course { Name = "Observación y sigilo operativo", Description = "Técnicas de observación, desplazamiento discreto y análisis del entorno.", Level = CourseLevel.Beginner, Price = 240, StartDate = new DateOnly(2026, 11, 9), EndDate = new DateOnly(2026, 12, 14), MaxStudents = 15, Status = CourseStatus.Open, AcademyId = institutoCamuflaje.Id },
                new Course { Name = "Interfaces web con componentes modernos", Description = "Construcción de interfaces mantenibles con componentes reutilizables.", Level = CourseLevel.Intermediate, Price = 320, StartDate = new DateOnly(2026, 11, 10), EndDate = new DateOnly(2027, 1, 12), MaxStudents = 24, Status = CourseStatus.Open, AcademyId = techMasters.Id },

                new Course { Name = "Finanzas para decisiones valientes", Description = "Presupuestos, tesorería y lectura de indicadores para pequeños negocios.", Level = CourseLevel.Beginner, Price = 220, StartDate = new DateOnly(2026, 11, 12), EndDate = new DateOnly(2026, 12, 17), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = businessAcademy.Id },
                new Course { Name = "SEO con brújula y palabras clave", Description = "Planificación de contenidos y posicionamiento orgánico con criterio.", Level = CourseLevel.Intermediate, Price = 260, StartDate = new DateOnly(2026, 11, 16), EndDate = new DateOnly(2026, 12, 21), MaxStudents = 16, Status = CourseStatus.Open, AcademyId = digitalSkillsHub.Id },
                new Course { Name = "Prototipos que cuentan historias", Description = "Diseño de experiencias y prototipos navegables centrados en las personas.", Level = CourseLevel.Intermediate, Price = 280, StartDate = new DateOnly(2026, 11, 18), EndDate = new DateOnly(2027, 1, 20), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = creativeDesignLab.Id },
                new Course { Name = "Código limpio para mentes curiosas", Description = "Bases sólidas de programación, pruebas y buenas prácticas.", Level = CourseLevel.Beginner, Price = 240, StartDate = new DateOnly(2026, 11, 23), EndDate = new DateOnly(2027, 1, 18), MaxStudents = 30, Status = CourseStatus.Open, AcademyId = futureCoders.Id },

                new Course { Name = "English for the next challenge", Description = "Inglés práctico para entrevistas, reuniones y proyectos internacionales.", Level = CourseLevel.Intermediate, Price = 230, StartDate = new DateOnly(2026, 11, 24), EndDate = new DateOnly(2027, 1, 26), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = globalLanguages.Id },
                new Course { Name = "Primeros auxilios en acción", Description = "Respuesta inicial ante emergencias habituales en el entorno laboral.", Level = CourseLevel.Beginner, Price = 190, StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2026, 12, 22), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = healthLearningCenter.Id },
                new Course { Name = "Mantenimiento industrial sin secretos", Description = "Principios de mantenimiento preventivo para instalaciones y maquinaria.", Level = CourseLevel.Intermediate, Price = 310, StartDate = new DateOnly(2026, 12, 2), EndDate = new DateOnly(2027, 1, 28), MaxStudents = 24, Status = CourseStatus.Open, AcademyId = industrialTraining.Id },
                new Course { Name = "Datos que explican el mundo", Description = "Introducción al análisis exploratorio y a la comunicación de resultados.", Level = CourseLevel.Beginner, Price = 300, StartDate = new DateOnly(2026, 12, 3), EndDate = new DateOnly(2027, 1, 21), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = dataScienceAcademy.Id },

                new Course { Name = "Energía solar desde el tejado", Description = "Diseño básico de instalaciones solares y cálculo de rendimiento.", Level = CourseLevel.Intermediate, Price = 270, StartDate = new DateOnly(2026, 12, 7), EndDate = new DateOnly(2027, 1, 25), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = greenEnergySchool.Id },
                new Course { Name = "Anfitriones que dejan huella", Description = "Comunicación, hospitalidad y resolución de situaciones con clientes.", Level = CourseLevel.Beginner, Price = 180, StartDate = new DateOnly(2026, 12, 9), EndDate = new DateOnly(2027, 1, 13), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = hospitalityExperts.Id },
                new Course { Name = "Defensa digital para equipos", Description = "Hábitos y controles esenciales para proteger cuentas y dispositivos.", Level = CourseLevel.Beginner, Price = 250, StartDate = new DateOnly(2026, 12, 10), EndDate = new DateOnly(2027, 1, 14), MaxStudents = 16, Status = CourseStatus.Open, AcademyId = cyberSecurityHub.Id },
                new Course { Name = "Ideas con rumbo de proyecto", Description = "De la intuición inicial a un plan de proyecto comprobable.", Level = CourseLevel.Beginner, Price = 210, StartDate = new DateOnly(2026, 12, 14), EndDate = new DateOnly(2027, 1, 18), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = innovationAcademy.Id },

                new Course { Name = "Entrevistas que encuentran talento", Description = "Preparación de entrevistas estructuradas y evaluación responsable.", Level = CourseLevel.Advanced, Price = 260, StartDate = new DateOnly(2027, 1, 11), EndDate = new DateOnly(2027, 2, 8), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = adeccoSchool.Id },
                new Course { Name = "Matemáticas para resolver problemas", Description = "Estrategias prácticas para razonar, estimar y comprobar resultados.", Level = CourseLevel.Beginner, Price = 170, StartDate = new DateOnly(2027, 1, 12), EndDate = new DateOnly(2027, 2, 9), MaxStudents = 32, Status = CourseStatus.Open, AcademyId = espanaEduca.Id },
                new Course { Name = "Camuflaje urbano y lectura del terreno", Description = "Prácticas de adaptación visual y orientación en entornos urbanos.", Level = CourseLevel.Intermediate, Price = 260, StartDate = new DateOnly(2027, 1, 13), EndDate = new DateOnly(2027, 2, 17), MaxStudents = 18, Status = CourseStatus.Open, AcademyId = institutoCamuflaje.Id },
                new Course { Name = "Servicios web con .NET", Description = "Diseño de APIs y servicios robustos para aplicaciones conectadas.", Level = CourseLevel.Advanced, Price = 360, StartDate = new DateOnly(2027, 1, 18), EndDate = new DateOnly(2027, 3, 2), MaxStudents = 18, Status = CourseStatus.Open, AcademyId = techMasters.Id },

                new Course { Name = "Liderazgo sereno en tiempos de cambio", Description = "Herramientas para coordinar equipos y tomar decisiones con claridad.", Level = CourseLevel.Intermediate, Price = 290, StartDate = new DateOnly(2027, 1, 20), EndDate = new DateOnly(2027, 2, 24), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = businessAcademy.Id },
                new Course { Name = "Campañas digitales con propósito", Description = "Planificación, segmentación y medición de campañas digitales.", Level = CourseLevel.Intermediate, Price = 280, StartDate = new DateOnly(2027, 1, 21), EndDate = new DateOnly(2027, 2, 25), MaxStudents = 18, Status = CourseStatus.Open, AcademyId = digitalSkillsHub.Id },
                new Course { Name = "Tipografía, color y jerarquía visual", Description = "Decisiones visuales para crear piezas claras, atractivas y coherentes.", Level = CourseLevel.Beginner, Price = 230, StartDate = new DateOnly(2027, 1, 25), EndDate = new DateOnly(2027, 2, 22), MaxStudents = 15, Status = CourseStatus.Open, AcademyId = creativeDesignLab.Id },
                new Course { Name = "Java para construir soluciones", Description = "Programación orientada a objetos y resolución de problemas reales.", Level = CourseLevel.Intermediate, Price = 310, StartDate = new DateOnly(2027, 1, 27), EndDate = new DateOnly(2027, 3, 10), MaxStudents = 24, Status = CourseStatus.Open, AcademyId = futureCoders.Id },

                new Course { Name = "Français pour voyager et travailler", Description = "Francés funcional para viajes, atención al público y trabajo.", Level = CourseLevel.Beginner, Price = 220, StartDate = new DateOnly(2027, 2, 1), EndDate = new DateOnly(2027, 3, 8), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = globalLanguages.Id },
                new Course { Name = "Bienestar y hábitos sostenibles", Description = "Rutinas saludables y prevención del desgaste en la vida diaria.", Level = CourseLevel.Beginner, Price = 160, StartDate = new DateOnly(2027, 2, 3), EndDate = new DateOnly(2027, 2, 24), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = healthLearningCenter.Id },
                new Course { Name = "Soldadura y unión de materiales", Description = "Principios de seguridad y técnicas básicas de unión industrial.", Level = CourseLevel.Intermediate, Price = 340, StartDate = new DateOnly(2027, 2, 8), EndDate = new DateOnly(2027, 3, 19), MaxStudents = 18, Status = CourseStatus.Open, AcademyId = industrialTraining.Id },
                new Course { Name = "Python para explorar datos", Description = "Automatización y análisis de datos con Python desde cero.", Level = CourseLevel.Intermediate, Price = 330, StartDate = new DateOnly(2027, 2, 10), EndDate = new DateOnly(2027, 3, 24), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = dataScienceAcademy.Id },

                new Course { Name = "Eficiencia energética en edificios", Description = "Medición del consumo y propuestas para reducir la huella energética.", Level = CourseLevel.Advanced, Price = 300, StartDate = new DateOnly(2027, 2, 15), EndDate = new DateOnly(2027, 3, 22), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = greenEnergySchool.Id },
                new Course { Name = "Diseño de experiencias gastronómicas", Description = "Creación de propuestas memorables para restaurantes y eventos.", Level = CourseLevel.Intermediate, Price = 250, StartDate = new DateOnly(2027, 2, 17), EndDate = new DateOnly(2027, 3, 17), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = hospitalityExperts.Id },
                new Course { Name = "Ethical Hacking: pensar como un atacante", Description = "Reconocimiento, amenazas y defensa desde una perspectiva responsable.", Level = CourseLevel.Advanced, Price = 390, StartDate = new DateOnly(2027, 2, 22), EndDate = new DateOnly(2027, 4, 5), MaxStudents = 16, Status = CourseStatus.Open, AcademyId = cyberSecurityHub.Id },
                new Course { Name = "Prototipado rápido de nuevos servicios", Description = "Experimentación, entrevistas y validación de ideas en poco tiempo.", Level = CourseLevel.Intermediate, Price = 270, StartDate = new DateOnly(2027, 2, 24), EndDate = new DateOnly(2027, 3, 31), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = innovationAcademy.Id },

                new Course { Name = "Nóminas y relaciones laborales", Description = "Conceptos esenciales para comprender la gestión laboral cotidiana.", Level = CourseLevel.Intermediate, Price = 240, StartDate = new DateOnly(2027, 3, 1), EndDate = new DateOnly(2027, 3, 29), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = adeccoSchool.Id },
                new Course { Name = "Lengua para comunicar mejor", Description = "Escritura, expresión oral y comprensión para contextos académicos.", Level = CourseLevel.Beginner, Price = 160, StartDate = new DateOnly(2027, 3, 3), EndDate = new DateOnly(2027, 3, 31), MaxStudents = 30, Status = CourseStatus.Open, AcademyId = espanaEduca.Id },
                new Course { Name = "Centro de mando y coordinación", Description = "Organización de equipos, comunicaciones y toma de decisiones bajo presión.", Level = CourseLevel.Advanced, Price = 320, StartDate = new DateOnly(2027, 3, 8), EndDate = new DateOnly(2027, 4, 12), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = institutoCamuflaje.Id },
                new Course { Name = "DevOps para equipos pequeños", Description = "Automatización, integración continua y entregas repetibles.", Level = CourseLevel.Advanced, Price = 370, StartDate = new DateOnly(2027, 3, 10), EndDate = new DateOnly(2027, 4, 21), MaxStudents = 30, Status = CourseStatus.Open, AcademyId = techMasters.Id },

                new Course { Name = "Emprender con números claros", Description = "Modelo de negocio, costes y decisiones financieras para emprender.", Level = CourseLevel.Intermediate, Price = 270, StartDate = new DateOnly(2027, 3, 15), EndDate = new DateOnly(2027, 4, 19), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = businessAcademy.Id },
                new Course { Name = "Analítica web para mejorar", Description = "Lectura de métricas y experimentos para optimizar resultados.", Level = CourseLevel.Advanced, Price = 310, StartDate = new DateOnly(2027, 3, 17), EndDate = new DateOnly(2027, 4, 14), MaxStudents = 18, Status = CourseStatus.Open, AcademyId = digitalSkillsHub.Id },
                new Course { Name = "Figma: del boceto al prototipo", Description = "Flujos, componentes y prototipos interactivos en Figma.", Level = CourseLevel.Intermediate, Price = 290, StartDate = new DateOnly(2027, 3, 22), EndDate = new DateOnly(2027, 4, 26), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = creativeDesignLab.Id },
                new Course { Name = "Python para automatizar tareas", Description = "Scripts útiles para ahorrar tiempo y trabajar con archivos y datos.", Level = CourseLevel.Beginner, Price = 280, StartDate = new DateOnly(2027, 3, 24), EndDate = new DateOnly(2027, 4, 28), MaxStudents = 20, Status = CourseStatus.Open, AcademyId = futureCoders.Id },

                new Course { Name = "Deutsch im digitalen Büro", Description = "Alemán práctico para correos, reuniones y herramientas de trabajo.", Level = CourseLevel.Intermediate, Price = 230, StartDate = new DateOnly(2027, 4, 5), EndDate = new DateOnly(2027, 5, 10), MaxStudents = 30, Status = CourseStatus.Open, AcademyId = globalLanguages.Id },
                new Course { Name = "Ciencia de datos para decisiones", Description = "Modelos sencillos y visualización para convertir datos en acción.", Level = CourseLevel.Advanced, Price = 380, StartDate = new DateOnly(2027, 4, 7), EndDate = new DateOnly(2027, 5, 19), MaxStudents = 25, Status = CourseStatus.Open, AcademyId = dataScienceAcademy.Id },
                new Course { Name = "Redes industriales conectadas", Description = "Comunicación entre dispositivos y fundamentos de automatización industrial.", Level = CourseLevel.Advanced, Price = 360, StartDate = new DateOnly(2027, 4, 12), EndDate = new DateOnly(2027, 5, 24), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = industrialTraining.Id },
                new Course { Name = "Innovación responsable y sostenible", Description = "Diseño de soluciones nuevas con impacto social y ambiental positivo.", Level = CourseLevel.Advanced, Price = 300, StartDate = new DateOnly(2027, 4, 14), EndDate = new DateOnly(2027, 5, 26), MaxStudents = 22, Status = CourseStatus.Open, AcademyId = innovationAcademy.Id }
            );

            context.SaveChanges();

            // Reparte los cursos anteriores entre los dos profesores demo (Id impar: uno, Id par: el otro).
            foreach (var course in context.Courses.ToList())
            {
                course.TeacherId = course.Id % 2 == 1 ? teacher1.Id : teacher2.Id;
            }

            // Cursos que ya empezaron o terminaron (para tener lecciones pasadas, notas y asistencias)
            // y dos borradores sin profesor: TeacherId es opcional y aquí se ven los dos casos.
            context.Courses.AddRange(
                new Course { Name = "Excel aplicado a recursos humanos", Description = "Hojas de cálculo para controlar plantillas, ausencias y costes de personal.", Level = CourseLevel.Beginner, Price = 150, StartDate = new DateOnly(2026, 9, 14), EndDate = new DateOnly(2026, 10, 5), MaxStudents = 20, Status = CourseStatus.Finished, AcademyId = adeccoSchool.Id, TeacherId = teacher1.Id },
                new Course { Name = "Fundamentos de SQL y bases de datos", Description = "Modelo relacional, consultas y diseño de tablas desde cero.", Level = CourseLevel.Beginner, Price = 200, StartDate = new DateOnly(2026, 9, 22), EndDate = new DateOnly(2026, 10, 20), MaxStudents = 20, Status = CourseStatus.Finished, AcademyId = techMasters.Id, TeacherId = teacher2.Id },
                new Course { Name = "Introducción a ASP.NET Core MVC", Description = "Controladores, vistas Razor y acceso a datos con Entity Framework Core.", Level = CourseLevel.Intermediate, Price = 290, StartDate = new DateOnly(2026, 10, 14), EndDate = new DateOnly(2026, 11, 25), MaxStudents = 18, Status = CourseStatus.InProgress, AcademyId = techMasters.Id, TeacherId = teacher1.Id },
                new Course { Name = "Inglés para el trabajo", Description = "Presentaciones, reuniones y correos profesionales en inglés.", Level = CourseLevel.Beginner, Price = 200, StartDate = new DateOnly(2026, 10, 15), EndDate = new DateOnly(2026, 12, 3), MaxStudents = 22, Status = CourseStatus.InProgress, AcademyId = globalLanguages.Id, TeacherId = teacher2.Id },
                new Course { Name = "Gestión del tiempo y productividad", Description = "Planificación semanal, prioridades y hábitos para trabajar con foco.", Level = CourseLevel.Beginner, Price = 120, StartDate = new DateOnly(2027, 5, 3), EndDate = new DateOnly(2027, 6, 7), MaxStudents = 25, Status = CourseStatus.Draft, AcademyId = businessAcademy.Id },
                new Course { Name = "Fotografía digital para redes sociales", Description = "Composición, luz y edición básica para contenidos de redes sociales.", Level = CourseLevel.Beginner, Price = 180, StartDate = new DateOnly(2027, 5, 10), EndDate = new DateOnly(2027, 6, 14), MaxStudents = 20, Status = CourseStatus.Draft, AcademyId = digitalSkillsHub.Id }
            );

            context.SaveChanges();
        }

        // Cursos sobre los que se crean las lecciones, matrículas, notas y asistencias de demo.
        var excelCourse = context.Courses.First(c => c.Name == "Excel aplicado a recursos humanos");
        var sqlCourse = context.Courses.First(c => c.Name == "Fundamentos de SQL y bases de datos");
        var aspnetCourse = context.Courses.First(c => c.Name == "Introducción a ASP.NET Core MVC");
        var englishCourse = context.Courses.First(c => c.Name == "Inglés para el trabajo");
        var peopleCourse = context.Courses.First(c => c.Name == "Primeros pasos en la gestión de personas");
        var historyCourse = context.Courses.First(c => c.Name == "Historia de España en movimiento");
        var webCourse = context.Courses.First(c => c.Name == "Interfaces web con componentes modernos");

        if (!context.Lessons.Any())
        {
            // Cada lección usa un aula de la misma academia que su curso. Dos lecciones futuras
            // no tienen aula todavía (ClassroomId es opcional), el resto sí.
            var primerAnio = context.Classrooms.First(c => c.Name == "Primer Año");
            var adeccoOnline = context.Classrooms.First(c => c.Name == "Adecco Online");
            var programacionWeb = context.Classrooms.First(c => c.Name == "Programación Web");
            var netAdvanced = context.Classrooms.First(c => c.Name == "NET Advanced");
            var angularLab = context.Classrooms.First(c => c.Name == "Angular Lab");
            var englishA1 = context.Classrooms.First(c => c.Name == "English A1");
            var historiaModerna = context.Classrooms.First(c => c.Name == "Historia Moderna");
            var aulaVirtualEso = context.Classrooms.First(c => c.Name == "Aula Virtual ESO");

            context.Lessons.AddRange(
                // Excel aplicado a recursos humanos: curso finalizado, lunes de 18:00 a 20:00.
                new Lesson { Name = "Fórmulas y funciones básicas", Description = "Sumas, promedios y funciones condicionales aplicadas a plantillas.", StartDate = new DateTime(2026, 9, 14, 18, 0, 0), EndDate = new DateTime(2026, 9, 14, 20, 0, 0), CourseId = excelCourse.Id, ClassroomId = primerAnio.Id },
                new Lesson { Name = "Tablas dinámicas", Description = "Resumen de ausencias y costes por departamento.", StartDate = new DateTime(2026, 9, 21, 18, 0, 0), EndDate = new DateTime(2026, 9, 21, 20, 0, 0), CourseId = excelCourse.Id, ClassroomId = primerAnio.Id },
                new Lesson { Name = "Gráficos para informes", Description = "Gráficos claros para presentar datos de personal.", StartDate = new DateTime(2026, 9, 28, 18, 0, 0), EndDate = new DateTime(2026, 9, 28, 20, 0, 0), CourseId = excelCourse.Id, ClassroomId = primerAnio.Id },
                new Lesson { Name = "Caso práctico: cálculo de nóminas", Description = "Ejercicio final con una plantilla de ejemplo.", StartDate = new DateTime(2026, 10, 5, 18, 0, 0), EndDate = new DateTime(2026, 10, 5, 20, 0, 0), CourseId = excelCourse.Id, ClassroomId = primerAnio.Id },

                // Fundamentos de SQL: curso finalizado, martes de 18:00 a 20:00.
                new Lesson { Name = "Modelo relacional", Description = "Tablas, claves primarias y claves foráneas.", StartDate = new DateTime(2026, 9, 22, 18, 0, 0), EndDate = new DateTime(2026, 9, 22, 20, 0, 0), CourseId = sqlCourse.Id, ClassroomId = programacionWeb.Id },
                new Lesson { Name = "Consultas SELECT y JOIN", Description = "Filtros, ordenación y unión de tablas.", StartDate = new DateTime(2026, 10, 6, 18, 0, 0), EndDate = new DateTime(2026, 10, 6, 20, 0, 0), CourseId = sqlCourse.Id, ClassroomId = programacionWeb.Id },
                new Lesson { Name = "Diseño de una base de datos", Description = "Del enunciado al esquema de tablas.", StartDate = new DateTime(2026, 10, 20, 18, 0, 0), EndDate = new DateTime(2026, 10, 20, 20, 0, 0), CourseId = sqlCourse.Id, ClassroomId = programacionWeb.Id },

                // Introducción a ASP.NET Core MVC: curso en marcha, miércoles de 18:00 a 20:30.
                new Lesson { Name = "Controladores y rutas", Description = "Cómo llega una petición a una acción del controlador.", StartDate = new DateTime(2026, 10, 14, 18, 0, 0), EndDate = new DateTime(2026, 10, 14, 20, 30, 0), CourseId = aspnetCourse.Id, ClassroomId = netAdvanced.Id },
                new Lesson { Name = "Vistas Razor", Description = "Vistas tipadas, layouts y helpers de etiquetas.", StartDate = new DateTime(2026, 10, 21, 18, 0, 0), EndDate = new DateTime(2026, 10, 21, 20, 30, 0), CourseId = aspnetCourse.Id, ClassroomId = netAdvanced.Id },
                new Lesson { Name = "Entity Framework Core", Description = "DbContext, entidades y consultas con LINQ.", StartDate = new DateTime(2026, 10, 28, 18, 0, 0), EndDate = new DateTime(2026, 10, 28, 20, 30, 0), CourseId = aspnetCourse.Id, ClassroomId = netAdvanced.Id },
                new Lesson { Name = "Relaciones y claves foráneas", Description = "Asociaciones 1:N y comportamiento al borrar.", StartDate = new DateTime(2026, 11, 4, 18, 0, 0), EndDate = new DateTime(2026, 11, 4, 20, 30, 0), CourseId = aspnetCourse.Id, ClassroomId = netAdvanced.Id },
                new Lesson { Name = "Formularios y validación", Description = "Crear y editar entidades con validación en servidor.", StartDate = new DateTime(2026, 11, 11, 18, 0, 0), EndDate = new DateTime(2026, 11, 11, 20, 30, 0), CourseId = aspnetCourse.Id, ClassroomId = netAdvanced.Id },
                new Lesson { Name = "Identity y roles", Description = "Login, cuentas de usuario y permisos por rol.", StartDate = new DateTime(2026, 11, 18, 18, 0, 0), EndDate = new DateTime(2026, 11, 18, 20, 30, 0), CourseId = aspnetCourse.Id },

                // Inglés para el trabajo: curso en marcha, jueves de 16:00 a 17:30.
                new Lesson { Name = "Presentaciones profesionales", Description = "Cómo presentarse y presentar un proyecto en inglés.", StartDate = new DateTime(2026, 10, 15, 16, 0, 0), EndDate = new DateTime(2026, 10, 15, 17, 30, 0), CourseId = englishCourse.Id, ClassroomId = englishA1.Id },
                new Lesson { Name = "Reuniones y llamadas", Description = "Vocabulario y frases útiles para reuniones.", StartDate = new DateTime(2026, 10, 22, 16, 0, 0), EndDate = new DateTime(2026, 10, 22, 17, 30, 0), CourseId = englishCourse.Id, ClassroomId = englishA1.Id },
                new Lesson { Name = "Correos profesionales", Description = "Estructura y tono de un correo de trabajo.", StartDate = new DateTime(2026, 10, 29, 16, 0, 0), EndDate = new DateTime(2026, 10, 29, 17, 30, 0), CourseId = englishCourse.Id, ClassroomId = englishA1.Id },
                new Lesson { Name = "Entrevistas de trabajo", Description = "Preguntas habituales y cómo responderlas.", StartDate = new DateTime(2026, 11, 12, 16, 0, 0), EndDate = new DateTime(2026, 11, 12, 17, 30, 0), CourseId = englishCourse.Id, ClassroomId = englishA1.Id },

                // Primeros pasos en la gestión de personas: acaba de empezar (martes y jueves, 18:00 a 20:00).
                new Lesson { Name = "Introducción a la gestión de personas", Description = "Qué hace un departamento de recursos humanos.", StartDate = new DateTime(2026, 11, 3, 18, 0, 0), EndDate = new DateTime(2026, 11, 3, 20, 0, 0), CourseId = peopleCourse.Id, ClassroomId = primerAnio.Id },
                new Lesson { Name = "Selección y acogida", Description = "Cómo se incorpora a una persona nueva al equipo.", StartDate = new DateTime(2026, 11, 5, 18, 0, 0), EndDate = new DateTime(2026, 11, 5, 20, 0, 0), CourseId = peopleCourse.Id, ClassroomId = adeccoOnline.Id },
                new Lesson { Name = "Desarrollo profesional", Description = "Planes de formación y carrera.", StartDate = new DateTime(2026, 11, 10, 18, 0, 0), EndDate = new DateTime(2026, 11, 10, 20, 0, 0), CourseId = peopleCourse.Id, ClassroomId = primerAnio.Id },
                new Lesson { Name = "Evaluación del desempeño", Description = "Objetivos, indicadores y conversaciones de seguimiento.", StartDate = new DateTime(2026, 11, 12, 18, 0, 0), EndDate = new DateTime(2026, 11, 12, 20, 0, 0), CourseId = peopleCourse.Id },

                // Historia de España en movimiento: acaba de empezar (jueves de 10:00 a 12:00).
                new Lesson { Name = "España a comienzos del siglo XX", Description = "Cambios sociales y políticos de principios de siglo.", StartDate = new DateTime(2026, 11, 5, 10, 0, 0), EndDate = new DateTime(2026, 11, 5, 12, 0, 0), CourseId = historyCourse.Id, ClassroomId = historiaModerna.Id },
                new Lesson { Name = "La Segunda República", Description = "Reformas y debates de la década de 1930.", StartDate = new DateTime(2026, 11, 12, 10, 0, 0), EndDate = new DateTime(2026, 11, 12, 12, 0, 0), CourseId = historyCourse.Id, ClassroomId = historiaModerna.Id },
                new Lesson { Name = "La Transición", Description = "El paso a la democracia y la Constitución de 1978.", StartDate = new DateTime(2026, 11, 19, 10, 0, 0), EndDate = new DateTime(2026, 11, 19, 12, 0, 0), CourseId = historyCourse.Id, ClassroomId = aulaVirtualEso.Id },

                // Interfaces web con componentes modernos: empieza la semana que viene (martes de 10:00 a 12:00).
                new Lesson { Name = "Componentes y plantillas", Description = "Qué es un componente y cómo se reutiliza.", StartDate = new DateTime(2026, 11, 10, 10, 0, 0), EndDate = new DateTime(2026, 11, 10, 12, 0, 0), CourseId = webCourse.Id, ClassroomId = angularLab.Id },
                new Lesson { Name = "Estado y eventos", Description = "Cómo reacciona un componente a las acciones del usuario.", StartDate = new DateTime(2026, 11, 17, 10, 0, 0), EndDate = new DateTime(2026, 11, 17, 12, 0, 0), CourseId = webCourse.Id, ClassroomId = angularLab.Id }
            );

            context.SaveChanges();
        }

        if (!context.Enrollments.Any())
        {
            // 15 matrículas: ningún alumno repite curso. Hay matrículas completadas, activas,
            // pendientes de pago y una abandonada, para ver todos los estados.
            context.Enrollments.AddRange(
                new Enrollment { StudentId = student1.Id, CourseId = excelCourse.Id, EnrolledAt = new DateTime(2026, 9, 1, 10, 0, 0), IsPaid = true, Status = EnrollmentStatus.Completed },
                new Enrollment { StudentId = student2.Id, CourseId = excelCourse.Id, EnrolledAt = new DateTime(2026, 9, 3, 12, 30, 0), IsPaid = true, Status = EnrollmentStatus.Completed },
                new Enrollment { StudentId = student3.Id, CourseId = excelCourse.Id, EnrolledAt = new DateTime(2026, 9, 5, 9, 15, 0), IsPaid = true, Status = EnrollmentStatus.Dropped },

                new Enrollment { StudentId = student1.Id, CourseId = sqlCourse.Id, EnrolledAt = new DateTime(2026, 9, 8, 11, 0, 0), IsPaid = true, Status = EnrollmentStatus.Completed },
                new Enrollment { StudentId = student3.Id, CourseId = sqlCourse.Id, EnrolledAt = new DateTime(2026, 9, 10, 16, 45, 0), IsPaid = true, Status = EnrollmentStatus.Completed },

                new Enrollment { StudentId = student1.Id, CourseId = aspnetCourse.Id, EnrolledAt = new DateTime(2026, 9, 28, 10, 20, 0), IsPaid = true, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = student2.Id, CourseId = aspnetCourse.Id, EnrolledAt = new DateTime(2026, 9, 30, 18, 5, 0), IsPaid = true, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = student3.Id, CourseId = aspnetCourse.Id, EnrolledAt = new DateTime(2026, 10, 5, 9, 40, 0), IsPaid = false, Status = EnrollmentStatus.Active },

                new Enrollment { StudentId = student2.Id, CourseId = englishCourse.Id, EnrolledAt = new DateTime(2026, 10, 5, 13, 0, 0), IsPaid = true, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = student3.Id, CourseId = englishCourse.Id, EnrolledAt = new DateTime(2026, 10, 8, 17, 30, 0), IsPaid = true, Status = EnrollmentStatus.Active },

                new Enrollment { StudentId = student1.Id, CourseId = peopleCourse.Id, EnrolledAt = new DateTime(2026, 10, 20, 10, 0, 0), IsPaid = true, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = student2.Id, CourseId = peopleCourse.Id, EnrolledAt = new DateTime(2026, 11, 2, 19, 10, 0), IsPaid = false, Status = EnrollmentStatus.Pending },

                new Enrollment { StudentId = student1.Id, CourseId = historyCourse.Id, EnrolledAt = new DateTime(2026, 10, 28, 12, 0, 0), IsPaid = true, Status = EnrollmentStatus.Active },
                new Enrollment { StudentId = student3.Id, CourseId = historyCourse.Id, EnrolledAt = new DateTime(2026, 11, 4, 8, 30, 0), IsPaid = false, Status = EnrollmentStatus.Pending },

                new Enrollment { StudentId = student2.Id, CourseId = webCourse.Id, EnrolledAt = new DateTime(2026, 11, 4, 20, 0, 0), IsPaid = false, Status = EnrollmentStatus.Pending }
            );

            context.SaveChanges();
        }

        if (!context.Grades.Any())
        {
            // Notas solo de matrículas con curso avanzado o terminado. Algunas sin comentario (es opcional).
            context.Grades.AddRange(
                // Excel aplicado a recursos humanos (finalizado)
                new Grade { EnrollmentId = FindEnrollment(context, student1, excelCourse).Id, Value = 8.5, GradeType = GradeType.Exam, GradedAt = new DateTime(2026, 9, 29), Comments = "Buen dominio de las fórmulas." },
                new Grade { EnrollmentId = FindEnrollment(context, student1, excelCourse).Id, Value = 9.0, GradeType = GradeType.Final, GradedAt = new DateTime(2026, 10, 8), Comments = "Excelente trabajo final." },
                new Grade { EnrollmentId = FindEnrollment(context, student2, excelCourse).Id, Value = 7.0, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 9, 28) },
                new Grade { EnrollmentId = FindEnrollment(context, student2, excelCourse).Id, Value = 7.5, GradeType = GradeType.Final, GradedAt = new DateTime(2026, 10, 8), Comments = "Aprobado con buena evolución." },
                new Grade { EnrollmentId = FindEnrollment(context, student3, excelCourse).Id, Value = 3.5, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 9, 22), Comments = "Entrega incompleta." },

                // Fundamentos de SQL (finalizado)
                new Grade { EnrollmentId = FindEnrollment(context, student1, sqlCourse).Id, Value = 9.5, GradeType = GradeType.Exam, GradedAt = new DateTime(2026, 10, 6), Comments = "Consultas muy bien resueltas." },
                new Grade { EnrollmentId = FindEnrollment(context, student1, sqlCourse).Id, Value = 9.2, GradeType = GradeType.Final, GradedAt = new DateTime(2026, 10, 21) },
                new Grade { EnrollmentId = FindEnrollment(context, student3, sqlCourse).Id, Value = 6.0, GradeType = GradeType.Exam, GradedAt = new DateTime(2026, 10, 6), Comments = "Repasar los JOIN." },
                new Grade { EnrollmentId = FindEnrollment(context, student3, sqlCourse).Id, Value = 6.5, GradeType = GradeType.Final, GradedAt = new DateTime(2026, 10, 21) },

                // Introducción a ASP.NET Core MVC (en marcha)
                new Grade { EnrollmentId = FindEnrollment(context, student1, aspnetCourse).Id, Value = 8.0, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 10, 22), Comments = "Controlador y vistas correctos." },
                new Grade { EnrollmentId = FindEnrollment(context, student1, aspnetCourse).Id, Value = 7.5, GradeType = GradeType.Exam, GradedAt = new DateTime(2026, 11, 4) },
                new Grade { EnrollmentId = FindEnrollment(context, student2, aspnetCourse).Id, Value = 9.5, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 10, 22), Comments = "Entrega muy cuidada." },
                new Grade { EnrollmentId = FindEnrollment(context, student3, aspnetCourse).Id, Value = 4.5, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 10, 29), Comments = "Faltan validaciones; hay que repetirla." },

                // Inglés para el trabajo (en marcha)
                new Grade { EnrollmentId = FindEnrollment(context, student2, englishCourse).Id, Value = 7.8, GradeType = GradeType.Assignment, GradedAt = new DateTime(2026, 10, 30) },
                new Grade { EnrollmentId = FindEnrollment(context, student3, englishCourse).Id, Value = 5.0, GradeType = GradeType.Exam, GradedAt = new DateTime(2026, 10, 30), Comments = "Aprobado justo; reforzar el vocabulario." }
            );

            context.SaveChanges();
        }

        if (!context.Attendances.Any())
        {
            // Asistencia solo a lecciones ya impartidas (hasta el 5 de noviembre) y de matrículas
            // que no están pendientes. Una fila por alumno y lección.
            var present = AttendanceStatus.Present;
            var absent = AttendanceStatus.Absent;
            var late = AttendanceStatus.Late;
            var excused = AttendanceStatus.Excused;

            // Excel (4 lecciones)
            var excelLessons = LessonsOf(context, excelCourse);
            AddAttendances(context, FindEnrollment(context, student1, excelCourse), excelLessons, present, present, present, present);
            AddAttendances(context, FindEnrollment(context, student2, excelCourse), excelLessons, present, late, present, present);
            AddAttendances(context, FindEnrollment(context, student3, excelCourse), excelLessons, present, absent, absent, absent);

            // SQL (3 lecciones)
            var sqlLessons = LessonsOf(context, sqlCourse);
            AddAttendances(context, FindEnrollment(context, student1, sqlCourse), sqlLessons, present, present, excused);
            AddAttendances(context, FindEnrollment(context, student3, sqlCourse), sqlLessons, present, late, present);

            // ASP.NET (las 4 primeras lecciones; las 2 últimas aún no se han dado)
            var aspnetLessons = LessonsOf(context, aspnetCourse);
            AddAttendances(context, FindEnrollment(context, student1, aspnetCourse), aspnetLessons, present, present, present, present);
            AddAttendances(context, FindEnrollment(context, student2, aspnetCourse), aspnetLessons, present, present, late, present);
            AddAttendances(context, FindEnrollment(context, student3, aspnetCourse), aspnetLessons, present, absent, present, absent);

            // Inglés (las 3 primeras lecciones)
            var englishLessons = LessonsOf(context, englishCourse);
            AddAttendances(context, FindEnrollment(context, student2, englishCourse), englishLessons, present, present, present);
            AddAttendances(context, FindEnrollment(context, student3, englishCourse), englishLessons, present, excused, present);

            // Cursos que acaban de empezar: solo el alumno con matrícula activa y las lecciones ya dadas
            AddAttendances(context, FindEnrollment(context, student1, peopleCourse), LessonsOf(context, peopleCourse), present, present);
            AddAttendances(context, FindEnrollment(context, student1, historyCourse), LessonsOf(context, historyCourse), present);

            context.SaveChanges();
        }
    }

    /// <summary>Busca la matrícula de un alumno en un curso (en los datos demo hay una por pareja).</summary>
    private static Enrollment FindEnrollment(ApplicationDbContext context, ApplicationUser student, Course course)
    {
        return context.Enrollments.First(e => e.StudentId == student.Id && e.CourseId == course.Id);
    }

    /// <summary>Devuelve las lecciones de un curso ordenadas por fecha.</summary>
    private static List<Lesson> LessonsOf(ApplicationDbContext context, Course course)
    {
        return context.Lessons.Where(l => l.CourseId == course.Id).OrderBy(l => l.StartDate).ToList();
    }

    /// <summary>Apunta la asistencia de una matrícula a las primeras lecciones, una por cada estado indicado.</summary>
    private static void AddAttendances(
        ApplicationDbContext context,
        Enrollment enrollment,
        List<Lesson> lessons,
        params AttendanceStatus[] statuses)
    {
        for (var i = 0; i < statuses.Length; i++)
        {
            context.Attendances.Add(new Attendance { LessonId = lessons[i].Id, EnrollmentId = enrollment.Id, Status = statuses[i] });
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
