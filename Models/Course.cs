using Microsoft.AspNetCore.Identity;

namespace G2AspNet.Models;

public class Course
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly DurationFrom { get; set; }
    public DateOnly DurationTo { get; set; }
    public int AcademyId { get; set; }
    // public List<Lesson> Lessons { get; set; } = new();
}