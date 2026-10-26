namespace G2AspNet.Models;

public class Classroom
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string? MeetingUrl { get; set; } // Solo si el aula es virtual.
    public bool IsActive { get; set; }
    
    // Relación FK con Academy.
    public int AcademyId { get; set; }
}