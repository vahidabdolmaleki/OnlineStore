using Shop.Application.DTOs.Person.PersonType;

namespace Shop.Application.Interfaces;

public interface IPersonTypeService
{
    Task<IReadOnlyList<PersonTypeDto>> GetAllAsync();
    Task<PersonTypeDto?> GetByIdAsync(int id);
    Task<PersonTypeDto> CreateAsync(CreatePersonTypeDto dto);
    Task<PersonTypeDto?> UpdateAsync(int id, UpdatePersonTypeDto dto);
    Task<bool> DeleteAsync(int id);
}
