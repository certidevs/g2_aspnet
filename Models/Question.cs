using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Question
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El texto de la pregunta es obligatorio")]
    [StringLength(1000, ErrorMessage = "La pregunta no puede superar los 1000 caracteres")]
    public string Text { get; set; } = string.Empty;

    [Required(ErrorMessage = "La opción 1 es obligatoria")]
    [StringLength(500, ErrorMessage = "La opción no puede superar los 500 caracteres")]
    public string Option1 { get; set; } = string.Empty;

    [Required(ErrorMessage = "La opción 2 es obligatoria")]
    [StringLength(500, ErrorMessage = "La opción no puede superar los 500 caracteres")]
    public string Option2 { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La opción no puede superar los 500 caracteres")]
    public string? Option3 { get; set; }

    [StringLength(500, ErrorMessage = "La opción no puede superar los 500 caracteres")]
    public string? Option4 { get; set; }

    [Range(1, 4, ErrorMessage = "Debe seleccionar una opción correcta entre 1 y 4")]
    public int CorrectOption { get; set; } = 1;

    // FKs
    public int ExamId { get; set; }
    public Exam? Exam { get; set; }
}
