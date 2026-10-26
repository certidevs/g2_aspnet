using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum GradeType
{
    [Display(Name = "Examen")]
    Exam,

    [Display(Name = "Tarea")]
    Assignment,

    [Display(Name = "Proyecto")]
    Project,
    
    [Display(Name = "Final")]
    Final
}
