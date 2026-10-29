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
    public CourseLevel Level { get; set; }
    public decimal Price { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public int MaxStudents { get; set; }
    public CourseStatus Status { get; set; }
    public int AcademyId { get; set; }
    public string? TeacherId { get; set; }

    // public List<Lesson> Lessons { get; set; } = new();
}