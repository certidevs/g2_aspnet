using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class ExamAnswer
{
    public int Id { get; set; }

    [Range(1, 4, ErrorMessage = "Debe seleccionar una opción entre 1 y 4")]
    public int SelectedOption { get; set; }

    public bool IsCorrect { get; set; }

    // Fks
    public int ExamAttemptId { get; set; }
    public ExamAttempt? ExamAttempt { get; set; }

    public int QuestionId { get; set; }
    public Question? Question { get; set; }
}
