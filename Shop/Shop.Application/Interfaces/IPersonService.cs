using Shop.Application.DTOs.Person;

namespace Shop.Application.Interfaces;

public interface IPersonService
{
    Task<IReadOnlyList<PersonDto>> GetAllAsync();
    Task<PersonDto?> GetByIdAsync(int id);
    Task<PersonDto> CreateAsync(CreatePersonDto dto);
    Task<PersonDto?> UpdateAsync(int id, UpdatePersonDto dto);
    Task<bool> DeleteAsync(int id);
}
