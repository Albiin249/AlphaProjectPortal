using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class ProjectMemberEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ForeignKey(nameof(Project))]
    public string? ProjectId { get; set; }
    public virtual ProjectEntity? Project { get; set; }

    [ForeignKey(nameof(User))]
    public string? UserId { get; set; }
    public virtual UserEntity? User { get; set; } 
}

