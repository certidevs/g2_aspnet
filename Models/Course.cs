using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Course
{
    public int Id { get; set; }
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre del curso es obligatorio")]
    [StringLength(40, ErrorMessage = "El nombre del curso no puede ser mayor a 40 caracteres")]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DurationFrom { get; set; }
    public DateOnly DurationTo { get; set; }
    public int AcademyId { get; set; }
    // public List<Lesson> Lessons { get; set; } = new();
}