using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum GradeEnum
{
    [Display(Name = "Sobresaliente")]
    GradeA,

    [Display(Name = "Notable")]
    GradeB,

    [Display(Name = "Suficiente")]
    GradeC,
    
    [Display(Name = "Insuficiente")]
    GradeD
}
