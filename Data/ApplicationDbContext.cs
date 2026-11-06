using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using G2AspNet.Models;

namespace G2AspNet.Data;

/// <summary>
/// Contexto de EF Core: configura SQLite y reúne las tablas de la aplicación.
/// Las entidades del dominio del proyecto se añadirán aquí.
/// </summary>
public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>EF Core recibe estas opciones desde Program.cs para conectar con SQLite.</summary>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// AQUI van los db que creamos
    public DbSet<Academy> Academies => Set<Academy>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Classroom> Classrooms => Set<Classroom>();
    public DbSet<Grade> Grades => Set<Grade>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();


    /// <summary>
    /// Configura las columnas de Identity y todas las relaciones del dominio. Cada relación declara
    /// su OnDelete explícito: Cascade solo si el hijo no tiene sentido sin el padre, Restrict si es
    /// historial o dato importante, SetNull solo si la clave foránea es opcional (int? / string?).
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(user => user.DisplayName).HasMaxLength(100);
            entity.Property(user => user.AvatarFileName).HasMaxLength(260);
            entity.Property(user => user.CreatedAt).IsRequired();
        });

        // Restrict: un curso es un dato importante; no se puede borrar una academia que aún tiene cursos.
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Academy)
            .WithMany(a => a.Courses)
            .HasForeignKey(c => c.AcademyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: las aulas se usan en lecciones; no se borra una academia que aún tiene aulas.
        modelBuilder.Entity<Classroom>()
            .HasOne(c => c.Academy)
            .WithMany(a => a.Classrooms)
            .HasForeignKey(c => c.AcademyId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: toda relación con ApplicationUser es Restrict; no se borra un profesor con cursos.
        modelBuilder.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany()
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cascade: una lección es parte del curso y no tiene sentido sin él (si el curso se borra, sus lecciones también).
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Course)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        // SetNull: ClassroomId es opcional (int?); si se borra el aula, la lección se queda sin aula asignada.
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.Classroom)
            .WithMany(c => c.Lessons)
            .HasForeignKey(l => l.ClassroomId)
            .OnDelete(DeleteBehavior.SetNull);

        // Restrict: toda relación con ApplicationUser es Restrict; no se borra un alumno con matrículas.
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Student)
            .WithMany()
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: la matrícula es historial y dinero cobrado; no se borra un curso con matrículas.
        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict: las notas son historial académico; una matrícula con notas no se borra (se marca como Abandonada).
        modelBuilder.Entity<Grade>()
            .HasOne(g => g.Enrollment)
            .WithMany(e => e.Grades)
            .HasForeignKey(g => g.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cascade: la asistencia es una línea de la lista de su lección; sin la lección no tiene sentido.
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Lesson)
            .WithMany(l => l.Attendances)
            .HasForeignKey(a => a.LessonId)
            .OnDelete(DeleteBehavior.Cascade);

        // Restrict: las asistencias son historial del alumno; una matrícula con asistencias no se borra.
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Enrollment)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EnrollmentId)
            .OnDelete(DeleteBehavior.Restrict);
    }

}
