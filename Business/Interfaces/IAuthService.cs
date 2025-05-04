using Business.Models;
using Domain.Dtos;

namespace Business.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> SignInAsync(SignInFormData formData);
        Task<AuthResult> SignOutAsync();
        Task<AuthResult> SignUpAsync(SignUpFormData formData);
    }
}