﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

[Index(nameof(ClientName), IsUnique = true)]
public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public virtual ICollection<ProjectEntity> Projects { get; set; } = [];
}
