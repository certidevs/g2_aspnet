namespace G2AspNet.Models;

public class Academy
{
    public int Id { get; set; }
    public required string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
}