using System.ComponentModel.DataAnnotations;

namespace G2AspNet.Models;

public class Academy
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la academia es obligatorio.")]
    [StringLength(30, MinimumLength = 2)]
    public required string Name { get; set; } = string.Empty;

    [StringLength(200, MinimumLength = 10)]
    public string? Description { get; set; }
    public string? Location { get; set; }
    public List<Course> Courses { get; set; } = new();
}