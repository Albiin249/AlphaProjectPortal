namespace Domain.Models;

public class ProjectMember
{
    public string? Id { get; set; }
    public string ProjectId { get; set; } = null!;
    public Project Project { get; set; } = null!;

    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}