namespace Data.Entities;

public class AddressEntity
{
    public int Id { get; set; }

    public string? Address { get; set; }

    public virtual ICollection<UserEntity> Users { get; set; } = [];
}
