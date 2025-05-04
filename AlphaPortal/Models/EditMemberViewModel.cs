using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AlphaPortal.Models;

public class EditMemberViewModel
{
    public string? Id { get; set; }

    [Required(ErrorMessage = "Required")]
    public string? FirstName { get; set; }

    [Required(ErrorMessage = "Required")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    [UniqueEmail]
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public string? ExistingProfileImagePath { get; set; }

    public string? BirthDay { get; set; }
    public string? BirthMonth { get; set; }
    public string? BirthYear { get; set; }

    public AddressModel? Address { get; set; }

}
