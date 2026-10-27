using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum EnrollmentStatus
{
    [Display(Name = "Pendiente")]
    Pending,

    [Display(Name = "Activo")]
    Active,

    [Display(Name = "Completado")]
    Completed,
    
    [Display(Name = "Abandonado")]
    Dropped
}
