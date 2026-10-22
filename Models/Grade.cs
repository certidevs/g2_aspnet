using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Grade
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;       //Me equivoque y este no deberia existir, pero ya hice la db con el asi que lo dejo sin uso

    [Required(ErrorMessage = "Debes proporcionar el nombre del alumno")]
    [StringLength(50, ErrorMessage = "El nombre no debe tener más de 50 caracteres")]
    public string NameStudent { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes proporcionar la nota")]
    [Range(0, 10, ErrorMessage = "La nota debe estar entre 0 y 10")]
    public double? GradeStudent { get; set; }

    [StringLength(3000, ErrorMessage = "La descripción no puede contener más de 3000 caracteres")]
    public string Description { get; set; } = string.Empty;



    public GradeEnum? GradeEnum
    {
        get
        {
            if (GradeStudent >= 9) return Models.GradeEnum.GradeA;
            if (GradeStudent >= 7) return Models.GradeEnum.GradeB;
            if (GradeStudent >= 5) return Models.GradeEnum.GradeC;
            return Models.GradeEnum.GradeD;
        }
    }

    //public List<Entity> Entitys { get; set; } = new();

}