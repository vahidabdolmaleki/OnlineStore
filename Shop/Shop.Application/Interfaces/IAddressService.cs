using Shop.Application.DTOs.Addresses;

namespace Shop.Application.Interfaces;

public interface IAddressService
{
    Task<IReadOnlyList<AddressDto>> GetAllAsync(int personId);
    Task<AddressDto> GetByIdAsync(int personId, int addressId);
    Task<AddressDto> CreateAsync(int personId, SaveAddressDto dto);
    Task<AddressDto> UpdateAsync(int personId, int addressId, SaveAddressDto dto);
    Task<AddressDto> SetDefaultAsync(int personId, int addressId);
    Task DeleteAsync(int personId, int addressId);
}
