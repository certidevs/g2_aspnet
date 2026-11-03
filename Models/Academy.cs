using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Academy
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la academia es obligatorio.")]
    [StringLength(30, MinimumLength = 2, ErrorMessage = "El nombre de la academia debe tener entre 2 y 30 caracteres.")]
    public required string Name { get; set; } = string.Empty;
    
    [StringLength(200, MinimumLength = 20, ErrorMessage = "Por favor proporcione una descripción más detallada.")]
    public string? Description { get; set; }
    public string? Location { get; set; }
    public List<Classroom> Classrooms { get; set; } = [];
    public List<Course> Courses { get; set; } = [];
}