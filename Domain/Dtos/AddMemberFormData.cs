namespace Domain.Dtos;

public class AddMemberFormData
{
    public string? Id { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? ProfileImage { get; set; }

    public string? BirthDay { get; set; }
    public string? BirthMonth { get; set; }
    public string? BirthYear { get; set; }

    public int? AddressId { get; set; }
    public AddAddressFormData? Address { get; set; }

}
