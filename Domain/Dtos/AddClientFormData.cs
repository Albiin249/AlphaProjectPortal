
namespace Domain.Dtos;

public class AddClientFormData
{
    public string? Id { get; set; }
    public string ClientName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}
