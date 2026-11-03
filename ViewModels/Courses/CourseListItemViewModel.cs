using G2AspNet.Models;

namespace G2AspNet.ViewModels.Courses;

/// <summary>Una fila del listado de cursos.</summary>
public sealed class CourseListItemViewModel
{
    public Course Course { get; init; } = null!;
}
