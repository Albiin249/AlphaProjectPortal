using Data.Contexts;
using System.ComponentModel.DataAnnotations;

namespace AlphaPortal.Models;

//Tog hjälp från ChatGPT för att skapa en validering av email, så att det går skicka ut felmeddelande om att den emailen är redan registrerad.
public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not string email || string.IsNullOrWhiteSpace(email))
            return ValidationResult.Success;

        var dbContext = validationContext.GetService<AppDbContext>();
        if (dbContext == null)
            throw new InvalidOperationException("ApplicationDbContext not found");

        var exists = dbContext.Users.Any(u => u.Email == email);
        if (exists)
        {
            return new ValidationResult("This email is already registered.");
        }

        return ValidationResult.Success;
    }
}