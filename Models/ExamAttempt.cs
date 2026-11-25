using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class ExamAttempt
{
    public int Id { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    [Range(0, 10, ErrorMessage = "La nota debe estar entre 0 y 10")]
    public double? Score { get; set; }

    public bool IsCompleted { get; set; } = false;

    // Fks
    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }

    public int ExamId { get; set; }
    public Exam? Exam { get; set; }

    // Lists
    public List<ExamAnswer> Answers { get; set; } = new();
}
