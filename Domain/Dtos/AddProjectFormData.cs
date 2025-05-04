namespace Domain.Dtos;

public class AddProjectFormData
{
    public string? Id { get; set; }
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string ClientId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public int StatusId { get; set; }
    public List<string> MemberIds { get; set; } = new();
}
