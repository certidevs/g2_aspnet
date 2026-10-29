using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Attendance
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public int EnrollmentId { get; set; }
    [Display(Name = "Asistencia")]
    [Required(ErrorMessage = "Es obligatorio seleccionar un estado de asistencia")]
    public AttendanceStatus Status { get; set; }
}