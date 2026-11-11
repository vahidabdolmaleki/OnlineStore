using Shop.Application.DTOs.Person;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class PersonService(IUnitOfWork unitOfWork) : IPersonService
{
    public async Task<IReadOnlyList<PersonDto>> GetAllAsync()
    {
        var persons = await unitOfWork.Repository<Person>().GetAsync(x => x.IsActive);

        // واکشی دستی نوع کاربر برای نمایش در DTO
        var types = await unitOfWork.Repository<PersonType>().GetAllAsync();

        return persons.Select(p => MapToDto(p, types.FirstOrDefault(t => t.Id == p.PersonTypeId))).ToList();
    }

    public async Task<PersonDto?> GetByIdAsync(int id)
    {
        var person = await unitOfWork.Repository<Person>().GetByIdAsync(id);
        if (person is not { IsActive: true }) return null;

        var type = await unitOfWork.Repository<PersonType>().GetByIdAsync(person.PersonTypeId);
        return MapToDto(person, type);
    }

    public async Task<PersonDto> CreateAsync(CreatePersonDto dto)
    {
        // چک کردن وجود PersonType
        var type = await unitOfWork.Repository<PersonType>().GetByIdAsync(dto.PersonTypeId);
        if (type is not { IsActive: true })
            throw new InvalidOperationException("نوع کاربر نامعتبر است.");

        // چک کردن یکتایی شماره موبایل
        var existing = await unitOfWork.Repository<Person>().GetAsync(x => x.PhoneNumber == dto.PhoneNumber && x.IsActive);
        if (existing.Count > 0)
            throw new InvalidOperationException("کاربری با این شماره موبایل قبلاً ثبت شده است.");

        var person = new Person
        {
            PhoneNumber = dto.PhoneNumber,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            NationalCode = dto.NationalCode,
            PersonTypeId = dto.PersonTypeId,
            IsActive = true
        };

        await unitOfWork.Repository<Person>().AddAsync(person);
        await unitOfWork.CompleteAsync();

        return MapToDto(person, type);
    }

    public async Task<PersonDto?> UpdateAsync(int id, UpdatePersonDto dto)
    {
        var person = await unitOfWork.Repository<Person>().GetByIdAsync(id);
        if (person is not { IsActive: true }) return null;

        person.FirstName = dto.FirstName;
        person.LastName = dto.LastName;
        person.Email = dto.Email;
        person.NationalCode = dto.NationalCode;

        await unitOfWork.CompleteAsync();

        var type = await unitOfWork.Repository<PersonType>().GetByIdAsync(person.PersonTypeId);
        return MapToDto(person, type);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var person = await unitOfWork.Repository<Person>().GetByIdAsync(id);
        if (person is not { IsActive: true }) return false;

        person.IsActive = false;
        await unitOfWork.CompleteAsync();
        return true;
    }

    private static PersonDto MapToDto(Person p, PersonType? t) => new()
    {
        Id = p.Id,
        PhoneNumber = p.PhoneNumber,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = p.Email,
        Score = p.Score,
        PersonTypeId = p.PersonTypeId,
        PersonTypeTitle = t?.Title ?? "نامشخص"
    };
}
