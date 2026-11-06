using G2AspNet.Models;

namespace G2AspNet.ViewModels.Attendances;

/// <summary>Filtro y resultados de la administración de asistencias.</summary>
public sealed class AttendanceIndexViewModel
{
    public string? Search { get; init; }
    public List<AttendanceListItemViewModel> Attendances { get; init; } = [];
}
