using G2AspNet.Data;
using G2AspNet.Models;
using G2AspNet.Utilities;

namespace G2AspNet.Utilities;

/// <summary>
/// Reglas de negocio para notas: quién puede ver, crear, editar o borrar una calificación.
/// Centraliza la lógica para no repetir comprobaciones en varios controladores.
/// </summary>
public static class GradeRules
{
    /// <summary>Indica si un usuario puede ver una calificación dada.</summary>
    /// <param name="context">Contexto de base de datos.</param>
    /// <param name="grade">Calificación a comprobar.</param>
    /// <param name="userId">Id del usuario logueado.</param>
    /// <param name="isStaff">true si el usuario es Admin o Teacher.</param>
    /// <returns>true si puede verla, false en caso contrario.</returns>
    public static bool PuedeVer(ApplicationDbContext context, Grade grade, string userId, bool isStaff)
    {
        if (isStaff) return true;
        return grade?.Enrollment?.StudentId == userId;
    }

    /// <summary>Indica si un profesor (no admin) es el docente del curso dado.</param>
    public static bool EsDocenteDelCurso(ApplicationDbContext context, int courseId, string teacherId)
    {
        var course = context.Courses.FirstOrDefault(c => c.Id == courseId);
        return course != null && course.TeacherId == teacherId;
    }

    /// <summary>Indica si una matrícula está en un estado válido para calificar.</summary>
    public static bool MatriculaValida(ApplicationDbContext context, int enrollmentId)
    {
        var enrollment = context.Enrollments.FirstOrDefault(e => e.Id == enrollmentId);
        return enrollment != null && EnrollmentRules.PlaceTakingStatuses.Contains(enrollment.Status);
    }
}
