using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Academy
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre de la academia es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la academia debe tener entre 2 y 100 caracteres.")]
    public string Name { get; set; } = string.Empty;
    [StringLength(200, MinimumLength = 20, ErrorMessage = "Por favor proporcione una descripción más detallada.")]
    public string? Description { get; set; }
    [Required(ErrorMessage = "La dirección es obligatoria.")]
    [StringLength(200, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 200 caracteres.")]
    public string? Address { get; set; }
    [Required(ErrorMessage = "La ciudad es obligatoria.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "La ciudad debe tener entre 2 y 100 caracteres.")]
    public string? City { get; set; }
    [Phone(ErrorMessage = "Por favor ingrese un número de teléfono válido.")]
    public string? Phone { get; set; }
    //public string? Location { get; set; }

    public List<Classroom> Classrooms { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
}
