namespace AlphaPortal.Models;

public class StatusesViewModel
{
    public IEnumerable<StatusViewModel> Statuses { get; set; } = [];
    public StatusViewModel StatusViewModel { get; set; } = new();

}
