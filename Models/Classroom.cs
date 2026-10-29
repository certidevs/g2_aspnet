using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Classroom
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre del aula es obligatorio.")]
    [StringLength(30, MinimumLength = 2)]
    public required string Name { get; set; } = string.Empty;
    
    [Range(5, 50, ErrorMessage = "Un aula debe de tener entre {1} y {2} plazas de capacidad.")]
    public int Capacity { get; set; }

    [Url]
    public string? MeetingUrl { get; set; } // Solo si el aula es virtual.
    public bool IsActive { get; set; }
    
    // Relación FK con Academy.
    public int AcademyId { get; set; }
}