using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Classroom : IValidatableObject
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre del aula es obligatorio.")]
    [StringLength(30, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [Range(5, 50, ErrorMessage = "Un aula debe de tener entre {1} y {2} plazas de capacidad.")]
    public int Capacity { get; set; }
    public ClassroomType ClassroomType { get; set; }
    [Url]
    public string? MeetingUrl { get; set; } // Solo si el aula es virtual.
    public bool IsActive { get; set; } = true;

    public int AcademyId { get; set; }
    public Academy? Academy { get; set; }
    public List<Lesson> Lessons { get; set; } = [];

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (ClassroomType == ClassroomType.Online && string.IsNullOrWhiteSpace(MeetingUrl))
        {
            yield return new ValidationResult("La URL de reunión es obligatoria para aulas virtuales.", [nameof(MeetingUrl)]);
        }
    }
}
