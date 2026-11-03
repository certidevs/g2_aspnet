using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace G2AspNet.Models;

public class Enrollment
{
    public int Id { get; set; }

    public string StudentId { get; set; } = string.Empty;
    public ApplicationUser? Student { get; set; }

    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    public bool IsPaid { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Pending;

    public List<Grade> Grades { get; set; } = new();
    public List<Attendance> Attendances { get; set; } = new();
}
