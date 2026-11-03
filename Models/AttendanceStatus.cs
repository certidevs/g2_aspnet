using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum AttendanceStatus
{
    [Display(Name = "Presente")]
    Present,

    [Display(Name = "Ausente")]
    Absent,

    [Display(Name = "Tarde")]
    Late,

    [Display(Name = "Justificado")]
    Excused
}