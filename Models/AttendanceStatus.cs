using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;
// prueba commit
// prueba commit 2
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
