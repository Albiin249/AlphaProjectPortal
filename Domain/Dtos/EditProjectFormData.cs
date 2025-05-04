namespace Domain.Dtos;

public class EditProjectFormData
{
    public string? Id { get; set; }
    public string? Image { get; set; }
    public string? ProjectName { get; set; }
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public string? ClientId { get; set; }
    public string? UserId { get; set; }
    public int StatusId { get; set; }
    public List<string> MemberIds { get; set; } = new();
}
