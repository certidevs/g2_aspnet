using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum CourseStatus
{
    [Display(Name = "Borrador")]
    Draft,

    [Display(Name = "Abierto")]
    Open,

    [Display(Name = "En Progreso")]
    InProgress,

    [Display(Name = "Finalizado")]
    Finished,

    [Display(Name = "Cancelado")]
    Cancelled
}
