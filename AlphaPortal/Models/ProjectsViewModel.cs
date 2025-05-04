namespace AlphaPortal.Models;

public class ProjectsViewModel
{
    public IEnumerable<ProjectViewModel> Projects { get; set; } = [];
    public AddProjectViewModel AddProjectFormData { get; set; } = new();
    public EditProjectViewModel EditProjectFormData { get; set; } = new();

    public int TotalCount { get; set; }
    public int OngoingCount { get; set; }
    public int CompletedCount { get; set; }
}
