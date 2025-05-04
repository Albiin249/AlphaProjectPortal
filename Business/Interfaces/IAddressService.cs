using Business.Models;
using Domain.Dtos;

namespace Business.Interfaces
{
    public interface IAddressService
    {
        Task<AddressResult> CreateAddressAsync(AddAddressFormData formData);
        Task<AddressResult> DeleteAddressAsync(int id);
        Task<AddressResult> GetAddressByIdAsync(int id);
        Task<AddressResult> GetAddressesAsync();
        Task<AddressResult> UpdateAddressAsync(AddAddressFormData formData);
    }
}