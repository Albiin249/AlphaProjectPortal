using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace Data.Entities;

public class UserEntity : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? JobTitle { get; set; }

    public DateTime? DateOfBirth { get; set; }

    public string? ProfileImage { get; set; }
    public virtual ICollection<ProjectMemberEntity> ProjectMemberships { get; set; } = [];

    public int? AddressId { get; set; }
    public virtual AddressEntity? Address { get; set; }
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
    public ICollection<NotificationDismissedEntity> DismissedNotifications { get; set; } = [];
}
