using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos;

public class SignInFormData
{
    [DataType(DataType.EmailAddress)]
    [Display(Name="Email", Prompt = "Enter email address")]
    public string? Email { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter password")]
    public string? Password { get; set; }
    public bool IsPersistent { get; set; }
}
