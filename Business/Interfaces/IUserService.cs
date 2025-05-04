using Business.Models;
using Data.Entities;
using Domain.Dtos;
using System.Security.Claims;

namespace Business.Interfaces
{
    public interface IUserService
    {
        Task<UserResult> GetUsersAsync();
        Task<UserResult> AddUserToRoleAsync(string userId, string roleName);
        Task<UserResult> CreateUserAsync(SignUpFormData formData, string roleName = "User");
        Task<UserResult> CreateMemberAsync(AddMemberFormData formData, string roleName = "User");
        Task<UserResult> DeleteMemberAsync(string id);
        Task<UserResult> GetUserAsync(string id);
        Task<UserResult> UpdateUserAsync(AddMemberFormData model);
        string GetUserId(ClaimsPrincipal user);
        Task<string> GetDisplayNameAsync(string? userName);
    }
}