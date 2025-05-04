namespace AlphaPortal.Models;

public class MemberDisplayViewModel
{
    public Guid Id { get; set; }

    public string? FirstName { get; set; } 
    public string? LastName { get; set; } 
    public string? Email { get; set; } 
    public string? PhoneNumber { get; set; } 
    public string? JobTitle { get; set; }
    public string? ProfileImage { get; set; }
    public string? Address { get; set; }
    public int? BirthDay { get; set; }
    public int? BirthMonth { get; set; }
    public int? BirthYear { get; set; }
}