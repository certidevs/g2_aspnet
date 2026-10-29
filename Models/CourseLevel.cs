using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public enum CourseLevel
{
    [Display(Name = "Principiante")]
    Beginner,

    [Display(Name = "Intermedio")]
    Intermediate,

    [Display(Name = "Avanzado")]
    Advanced
}
