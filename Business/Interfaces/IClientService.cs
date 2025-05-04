using Business.Models;
using Data.Entities;
using Data.Models;
using Domain.Dtos;

namespace Business.Interfaces
{
    public interface IClientService
    {
        Task<ClientResult> CreateClientAsync(AddClientFormData formData);
        Task<ClientResult> DeleteClientAsync(string id);
        Task<SingleClientResult> GetClientByIdAsync(string id);
        Task<ClientResult> GetClientsAsync();
        Task<ClientResult> UpdateClientAsync(AddClientFormData formData);
    }
}