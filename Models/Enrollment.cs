using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;

namespace G2AspNet.Models;

public class Enrollment
{
    public int Id { get; set; }
    
    public DateTime? EnrolledAt { get; set; }

    public bool IsPaid { get; set; }

    public EnrollmentStatus? EnrollmentStatus { get; set; }


    // public int StudentId { get; set; }
    // public int CourseId { get; set; }


    //public List<Grade> Grades { get; set; } = new();
    //public List<Attendance> Attendances { get; set; } = new();
}