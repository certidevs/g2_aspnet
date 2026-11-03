using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Attendance
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }

    public int EnrollmentId { get; set; }
    public Enrollment? Enrollment { get; set; }

    [Display(Name = "Asistencia")]
    public AttendanceStatus Status { get; set; }
}
