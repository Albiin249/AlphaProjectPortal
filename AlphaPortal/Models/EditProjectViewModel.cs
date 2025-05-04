using Domain.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlphaPortal.Models;

public class EditProjectViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Required")]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public IFormFile? ImageFile { get; set; }
    public string? ExistingProfileImagePath { get; set; }
    public string? ClientId { get; set; } 
    public string? ClientName { get; set; }
    public int StatusId { get; set; }
    public List<string> SelectedUserIds { get; set; } = new();
    public string? SelectedStatus { get; set; }
    public List<User> PreSelectedUsers { get; set; } = new();

    public IEnumerable<SelectListItem> Clients { get; set; } = [];
    public IEnumerable<SelectListItem> Members { get; set; } = [];
    public IEnumerable<SelectListItem> Statuses { get; set; } = [];
}
