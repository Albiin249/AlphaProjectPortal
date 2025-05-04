using Business.Factories;
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Dtos;
using Domain.Extensions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;
using System.Security.Claims;
namespace Business.Services;

public class UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, RoleManager<IdentityRole> roleManager, IAddressService addressService) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly UserManager<UserEntity> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IAddressService _addressService = addressService;


    //CREATE
    public async Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName = "User")
    {
        if (formData == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

        var existsResult = await _userRepository.Exists(x => x.Email == formData.Email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "User with same email already exists." };

        try
        {
            var userEntity = UserFactory.ToEntity(formData);


            var result = await _userManager.CreateAsync(userEntity, formData.Password);
            if (result.Succeeded)
            {
                var addToRoleResult = await AddUserToRoleAsync(userEntity.Id, roleName);
                return result.Succeeded
                    ? new UserResult { Succeeded = true, StatusCode = 201 }
                    : new UserResult { Succeeded = false, StatusCode = 201, Error = "User created but not added to default role." };
            }

            return new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public async Task<UserResult> CreateMemberAsync(AddMemberFormData formData, string roleName = "User")
    {
        if (formData == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Form data cannot be null." };

        var existsResult = await _userRepository.Exists(x => x.Email == formData.Email);
        if (existsResult.Succeeded)
            return new UserResult { Succeeded = false, StatusCode = 409, Error = "User with same email already exists." };

        try
        {
            var address = await _addressService.CreateAddressAsync(formData.Address!);

            if (!address.Succeeded)
                return new UserResult { Succeeded = false, StatusCode = 400, Error = "Failed to create address." };

            var userEntity = UserFactory.MemberToEntity(formData);
            userEntity.AddressId = address.AddressId;

            var result = await _userManager.CreateAsync(userEntity);
            if (result.Succeeded)
            {
                var addToRoleResult = await AddUserToRoleAsync(userEntity.Id, roleName);
                return result.Succeeded
                    ? new UserResult { Succeeded = true, StatusCode = 201 }
                    : new UserResult { Succeeded = false, StatusCode = 201, Error = "User created but not added to default role." };
            }

            return new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to create user." };

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new UserResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    //READ
    public string GetUserId(ClaimsPrincipal user)
    {
        return _userManager.GetUserId(user)!;
    }
    public async Task<UserResult> GetUserAsync(string id)
    {
        var result = await _userRepository.GetAsync(x => x.Id == id);
        return new UserResult
        {
            Succeeded = result.Succeeded,
            Result = result.Result != null ? new List<User> { result.Result } : new List<User>()
        };
    }

    public async Task<UserResult> GetUsersAsync()
    {
        var result = await _userRepository.GetAllAsync(includes: u => u.Address!);

        return result.MapTo<UserResult>();
    }

    public async Task<string> GetDisplayNameAsync(string? userName)
    {
        if (userName == null)
            return "";

        var user = await _userManager.FindByNameAsync(userName);
        return user == null ? "" : $"{user.FirstName} {user.LastName}";
    }

    public async Task<UserResult> AddUserToRoleAsync(string userId, string roleName)
    {
        if (!await _roleManager.RoleExistsAsync(roleName))
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "Role does not exist." };

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User does not exist." };

        var result = await _userManager.AddToRoleAsync(user, roleName);
        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult { Succeeded = false, StatusCode = 500, Error = "Unable to add user to role." };
    }

    //UPDATE
    public async Task<UserResult> UpdateUserAsync(AddMemberFormData model)
    {
        if (model == null)
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var existingUser = await _userManager.FindByIdAsync(model.Id!.ToString());
        if (existingUser == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User not found." };

        existingUser.FirstName = model.FirstName;
        existingUser.LastName = model.LastName;
        existingUser.Email = model.Email;
        existingUser.UserName = model.Email;
        existingUser.JobTitle = model.JobTitle;
        if (model.Address != null && !string.IsNullOrWhiteSpace(model.Address.Address))
        {
            existingUser.Address = new AddressEntity
            {
                Address = model.Address.Address
            };
        }
        existingUser.PhoneNumber = model.PhoneNumber;
        existingUser.ProfileImage = model.ProfileImage;
        var result = await _userManager.UpdateAsync(existingUser);

        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult { Succeeded = false };
    }

    //DELETE
    public async Task<UserResult> DeleteMemberAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return new UserResult { Succeeded = false, StatusCode = 400, Error = "User/Member ID is required." };



        var clientEntity = await _userRepository.GetEntityAsync(c => c.Id == id);
        if (clientEntity == null)
            return new UserResult { Succeeded = false, StatusCode = 404, Error = "User/Member not found." };

        
        if (clientEntity.AddressId != null)
        {
            await _addressService.DeleteAddressAsync(clientEntity.AddressId.Value!);
            clientEntity.AddressId = null;
        }
            
        var result = await _userRepository.DeleteAsync(clientEntity);

        return result.Succeeded
            ? new UserResult { Succeeded = true, StatusCode = 200 }
            : new UserResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

}




