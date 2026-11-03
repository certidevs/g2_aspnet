using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Grade
{
    public int Id { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }

    [Required(ErrorMessage = "Debes proporcionar la nota")]
    [Range(0, 10, ErrorMessage = "La nota debe estar entre 0 y 10")]
    public double? Value { get; set; }

    [StringLength(3000, ErrorMessage = "Los comentarios no puede contener más de 3000 caracteres")]
    public string? Comments { get; set; }

    public DateTime? GradedAt { get; set; }

    public GradeType? GradeType { get; set; }

    public GradeEnum? GradeEnum
    {
        get
        {
            if (Value is null) return null;
            if (Value >= 9) return Models.GradeEnum.GradeA;
            if (Value >= 7) return Models.GradeEnum.GradeB;
            if (Value >= 5) return Models.GradeEnum.GradeC;
            return Models.GradeEnum.GradeD;
        }
    }

    //public List<Entity> Entitys { get; set; } = new();
}
