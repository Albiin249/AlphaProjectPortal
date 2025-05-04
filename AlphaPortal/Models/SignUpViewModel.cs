using Domain.Dtos;
using System.ComponentModel.DataAnnotations;

namespace AlphaPortal.Models;

public class SignUpViewModel
{
    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "First Name", Prompt = "Enter your first name")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name", Prompt = "Enter your Last name")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Invalid email address")]
    [DataType(DataType.EmailAddress)]
    [UniqueEmail]
    [Display(Name = "Email", Prompt = "Enter your email")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Invalid password")]
    [DataType(DataType.Password)]
    [Display(Name = "Password", Prompt = "Enter your password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "This field is required.")]
    [Compare(nameof(Password), ErrorMessage = "Password must be confirmed.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm Password", Prompt = "Confirm password")]
    public string ConfirmPassword { get; set; } = null!;

    [Display(Name = "Terms And Conditions", Prompt = "I accept the terms and conditions.")]
    [Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions to use this site.")]
    public bool Terms {  get; set; }

    public static implicit operator SignUpFormData(SignUpViewModel model)
    {
        return model == null
            ? null!
            : new SignUpFormData
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Password = model.Password
            };
    }
}
