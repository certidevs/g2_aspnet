using G2AspNet.Models;

namespace G2AspNet.ViewModels.Attendances;

/// <summary>Una fila del listado de asistencias.</summary>
public sealed class AttendanceListItemViewModel
{
    public Attendance Attendance { get; init; } = null!;
}
