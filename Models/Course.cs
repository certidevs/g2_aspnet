using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Course : IValidatableObject
{
    public int Id { get; set; }
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre del curso es obligatorio")]
    [StringLength(40, ErrorMessage = "El nombre del curso no puede ser mayor a 40 caracteres")]
    public string Name { get; set; } = string.Empty;
    [StringLength(500, ErrorMessage = "La descripción no puede ser mayor a 500 caracteres")]
    public string? Description { get; set; }
    public CourseLevel Level { get; set; }
    [Range(0, 10000, ErrorMessage = "El precio debe estar entre {1} y {2} euros")]
    public decimal Price { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    [Range(1, 200, ErrorMessage = "Las plazas máximas deben estar entre {1} y {2}")]
    public int MaxStudents { get; set; }
    public CourseStatus Status { get; set; }

    public int AcademyId { get; set; }
    public Academy? Academy { get; set; }

    // El profesor es opcional (string?): un curso en borrador puede no tener profesor asignado todavía.
    public string? TeacherId { get; set; }
    public ApplicationUser? Teacher { get; set; }

    public List<Lesson> Lessons { get; set; } = new();
    public List<Enrollment> Enrollments { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate < StartDate)
        {
            yield return new ValidationResult("La fecha de finalización no puede ser anterior a la de inicio.", [nameof(EndDate)]);
        }
    }
}
