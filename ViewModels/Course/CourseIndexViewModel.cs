using G2AspNet.Models;

namespace G2AspNet.ViewModels.Courses;

/// <summary>Filtro y resultados de la administración de cursos.</summary>
public sealed class CourseIndexViewModel
{
    public string? Search { get; init; }
    public List<CourseListItemViewModel> Users { get; init; } = [];
}
