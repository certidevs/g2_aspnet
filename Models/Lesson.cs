using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Lesson
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la lección es obligatorio")]
    [StringLength(40, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public int? ClassroomId { get; set; }
    public Classroom? Classroom { get; set; }
    public ICollection<Attendance> Attendances { get; set; }
        = new List<Attendance>();
}