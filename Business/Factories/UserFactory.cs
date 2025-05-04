using Data.Entities;
using Domain.Dtos;
using Domain.Models;

namespace Business.Factories;

public class UserFactory
{
    public static UserEntity ToEntity(SignUpFormData dto)
    {
        return dto == null
            ? new UserEntity()
            : new UserEntity()
            {
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
            };
    }


    public static User ToModel(UserEntity user)
    {
        return user == null
            ? new User()
            : new User()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                JobTitle = user.JobTitle,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address is not null ? new AddressModel
                {
                    Address = user.Address.Address,
                } : null,
            };
    }

    public static UserEntity MemberToEntity(AddMemberFormData dto)
    {
        return dto == null
            ? new UserEntity()
            : new UserEntity()
            {
                UserName = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                JobTitle = dto.JobTitle,
                ProfileImage = dto.ProfileImage,
                DateOfBirth = TryParseDate(dto.BirthYear, dto.BirthMonth, dto.BirthDay),
                AddressId = dto.AddressId
            };
    }
    private static DateTime? TryParseDate(string? year, string? month, string? day)
    {
        if (int.TryParse(year, out var y) && int.TryParse(month, out var m) && int.TryParse(day, out var d))
        {
            return new DateTime(y, m, d);
        }

        return null;
    }
}
