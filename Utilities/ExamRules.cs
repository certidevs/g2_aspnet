using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Utilities;

/// <summary>
/// Reglas de negocio para exámenes: quién puede hacer un intento, quién puede crear/editar, etc.
/// Centraliza la lógica para no repetir comprobaciones en varios controladores.
/// </summary>
public static class ExamRules
{
    /// <summary>Indica si un alumno puede hacer un intento en el examen dado.</summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="exam">Examen a comprobar.</param>
    /// <param name="studentId">Id del alumno.</param>
    /// <returns>true si puede hacer el intento, false en caso contrario.</returns>
    public static bool CanTakeExam(ApplicationDbContext context, Exam exam, string studentId)
    {
        if (exam == null) return false;

        // 1. Debe tener matrícula válida (Pending o Active) en el curso del examen
        var tieneMatricula = context.Enrollments.Any(e =>
            e.CourseId == exam.CourseId &&
            e.StudentId == studentId &&
            EnrollmentRules.PlaceTakingStatuses.Contains(e.Status));

        if (!tieneMatricula) return false;

        // 2. Debe estar dentro de las fechas de apertura y cierre
        var ahora = DateTime.UtcNow;
        if (exam.FechaApertura.HasValue && ahora < exam.FechaApertura.Value) return false;
        if (exam.FechaCierre.HasValue && ahora > exam.FechaCierre.Value) return false;

        // 3. No debe haber agotado los intentos máximos
        var intentosRealizados = context.ExamAttempts.Count(e =>
            e.ExamId == exam.Id && e.StudentId == studentId && e.IsCompleted);

        return intentosRealizados < exam.MaxIntentos;
    }

    /// <summary>Indica si un profesor (no admin) es el docente del curso del examen.</param>
    public static bool EsDocenteDelCurso(ApplicationDbContext context, int courseId, string teacherId)
    {
        var course = context.Courses.FirstOrDefault(c => c.Id == courseId);
        return course != null && course.TeacherId == teacherId;
    }
}
