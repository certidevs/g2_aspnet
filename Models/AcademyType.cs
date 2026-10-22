using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

enum AcademyType
{
    [Display(Name = "Escuela")]
    School,
    [Display(Name = "Universidad")]
    University,
    [Display(Name = "Centro de Formación")]
    TemporalCourse // Cursos de formación con duradas menores a un año académico
}