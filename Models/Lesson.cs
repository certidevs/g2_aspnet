using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Lesson : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la lección es obligatorio")]
    [StringLength(40, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [StringLength(500, ErrorMessage = "La descripción no puede ser mayor a 500 caracteres")]
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    // El aula es opcional (int?): una lección puede estar pendiente de asignar aula.
    public int? ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
    public ICollection<Attendance> Attendances { get; set; }
        = new List<Attendance>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult("La hora de fin debe ser posterior a la de inicio.", [nameof(EndDate)]);
        }
    }
}
