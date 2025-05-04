using Domain.Models;

namespace AlphaPortal.Models;

public class ClientsViewModel
{
    public IEnumerable<ClientViewModel> Clients { get; set; } = [];
    public AddClientViewModel AddClientFormData { get; set; } = new();
    public EditClientViewModel EditClientFormData { get; set; } = new();
}
