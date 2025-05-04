using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Domain.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Business.Services;

public class AuthService(IUserService userService, SignInManager<UserEntity> signInManager) : IAuthService
{
    private readonly IUserService _userService = userService;
    private readonly SignInManager<UserEntity> _signInManager = signInManager;

    public async Task<AuthResult> SignInAsync(SignInFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _signInManager.PasswordSignInAsync(formData.Email, formData.Password, formData.IsPersistent, false);
        return result.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = 401, Error = "Invalid email or password." };
    }

    public async Task<AuthResult> SignUpAsync(SignUpFormData formData)
    {
        if (formData == null)
            return new AuthResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };

        var result = await _userService.CreateUserAsync(formData);
        return result.Succeeded
            ? new AuthResult { Succeeded = true, StatusCode = 201 }
            : new AuthResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    public async Task<AuthResult> SignOutAsync()
    {
        await _signInManager.SignOutAsync();
        return new AuthResult { Succeeded = true, StatusCode = 201 };
    }
}