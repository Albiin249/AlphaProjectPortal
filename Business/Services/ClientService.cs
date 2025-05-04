using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Dtos;
using Domain.Extensions;

namespace Business.Services;

public class ClientService(IClientRepository clientRepository) : IClientService
{
    private readonly IClientRepository _clientRepository = clientRepository;

    //CREATE
    public async Task<ClientResult> CreateClientAsync(AddClientFormData formData)
    {
        if (formData == null)
            return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };
        if (string.IsNullOrEmpty(formData.Id))
            formData.Id = Guid.NewGuid().ToString();

        var clientEntity = formData.MapTo<ClientEntity>();
        var result = await _clientRepository.AddAsync(clientEntity);


        return result.Succeeded
            ? new ClientResult { Succeeded = true, StatusCode = 201 }
            : new ClientResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //READ
    public async Task<ClientResult> GetClientsAsync()
    {
        var result = await _clientRepository.GetAllAsync();
        return result.MapTo<ClientResult>();
    }
    public async Task<SingleClientResult> GetClientByIdAsync(string id)
    {
        var result = await _clientRepository.GetAsync(c => c.Id == id);
        if (id == null)
            return new SingleClientResult { Succeeded = false, StatusCode = 400, Error = "ID cannot be null." };

        return result.MapTo<SingleClientResult>();
    }

    //UPDATE
    public async Task<ClientResult> UpdateClientAsync(AddClientFormData formData)
    {
        if (formData == null)
            return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Not all required fields are supplied." };


        var updatedEntity = formData.MapTo<ClientEntity>();

        var result = await _clientRepository.UpdateAsync(updatedEntity);


        return result.Succeeded
            ? new ClientResult { Succeeded = true, StatusCode = 200 }
            : new ClientResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //DELETE
    public async Task<ClientResult> DeleteClientAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
            return new ClientResult { Succeeded = false, StatusCode = 400, Error = "Client ID is required." };
        

 
        var clientEntity = await _clientRepository.GetEntityAsync(c => c.Id == id);
        if (clientEntity == null)
            return new ClientResult { Succeeded = false, StatusCode = 404, Error = "Client not found." };
        
     
        var result = await _clientRepository.DeleteAsync(clientEntity);

        return result.Succeeded
            ? new ClientResult { Succeeded = true, StatusCode = 200 }
            : new ClientResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

}
