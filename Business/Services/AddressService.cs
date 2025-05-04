
using Business.Interfaces;
using Business.Models;
using Data.Entities;
using Data.Interfaces;
using Domain.Dtos;
using Domain.Extensions;

namespace Business.Services;

public class AddressService(IAddressRepository addressRepository) : IAddressService
{

    private readonly IAddressRepository _addressRepository = addressRepository;

    //CREATE
    public async Task<AddressResult> CreateAddressAsync(AddAddressFormData formData)
    {
        if (formData == null)
            return new AddressResult { Succeeded = false, StatusCode = 400, Error = "You need to fill in address information." };

        var entity = formData.MapTo<AddressEntity>();
        var result = await _addressRepository.AddAsync(entity);



        return result.Succeeded
            ? new AddressResult { Succeeded = true, StatusCode = 201, AddressId = entity.Id }
            : new AddressResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //READ
    public async Task<AddressResult> GetAddressesAsync()
    {
        var result = await _addressRepository.GetAllAsync();
        return result.MapTo<AddressResult>();
    }
    public async Task<AddressResult> GetAddressByIdAsync(int id)
    {
        var result = await _addressRepository.GetAsync(a => a.Id == id);
        return result.MapTo<AddressResult>();
    }

    //UPDATE
    public async Task<AddressResult> UpdateAddressAsync(AddAddressFormData formData)
    {
        if (formData == null)
            return new AddressResult { Succeeded = false, StatusCode = 400, Error = "You need to fill in address information." };


        var updatedEntity = formData.MapTo<AddressEntity>();

        var result = await _addressRepository.UpdateAsync(updatedEntity);


        return result.Succeeded
            ? new AddressResult { Succeeded = true, StatusCode = 200 }
            : new AddressResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }

    //DELETE
    public async Task<AddressResult> DeleteAddressAsync(int id)
    {
        var entity = await _addressRepository.GetEntityAsync(a => a.Id == id);
        if (entity == null)
            return new AddressResult { Succeeded = false, StatusCode = 404, Error = "Address not found." };


        var result = await _addressRepository.DeleteAsync(entity);

        return result.Succeeded
            ? new AddressResult { Succeeded = true, StatusCode = 200 }
            : new AddressResult { Succeeded = false, StatusCode = result.StatusCode, Error = result.Error };
    }
}
