using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Exam
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El título es obligatorio")]
    [StringLength(200, ErrorMessage = "El título no puede superar los 200 caracteres")]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres")]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    [Display(Name = "Fecha de apertura")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? FechaApertura { get; set; }

    [Display(Name = "Fecha de cierre")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
    public DateTime? FechaCierre { get; set; }

    [Range(1, 100, ErrorMessage = "El número de intentos debe estar entre 1 y 100")]
    public int MaxIntentos { get; set; } = 1;

    // Fks
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public string TeacherId { get; set; } = string.Empty;
    public ApplicationUser? Teacher { get; set; }

    // Lists (entities that have Exam as FK)
    public List<Question> Questions { get; set; } = new();
    public List<ExamAttempt> ExamAttempts { get; set; } = new();
}
