using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AlphaPortal.Models;

public class AddProjectViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Required")]
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal Budget { get; set; }
    public IFormFile? ImageFile { get; set; }

    [Required(ErrorMessage = "Required")]
    public string ClientId { get; set; } = null!;

    public List<string> SelectedUserIds { get; set; } = new();

    public IEnumerable<SelectListItem> Statuses { get; set; } = [];
    public IEnumerable<SelectListItem> Clients { get; set; } = [];
    public IEnumerable<SelectListItem> Members { get; set; } = [];

}
