using System.Linq;
using Shop.Application.DTOs.Person;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public class PersonTypeService : IPersonTypeService
{
    private readonly IUnitOfWork _unitOfWork;

    public PersonTypeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<PersonTypeDto>> GetAllAsync()
    {
        var types = await _unitOfWork.Repository<PersonType>()
            .GetAsync(x => x.IsActive);

        return types
            .OrderBy(x => x.Id)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<PersonTypeDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Repository<PersonType>()
            .GetByIdAsync(id);

        return entity is { IsActive: true }
            ? MapToDto(entity)
            : null;
    }

    public async Task<PersonTypeDto> CreateAsync(CreatePersonTypeDto dto)
    {
        var title = dto.Title?.Trim();
        var code = dto.Code?.Trim().ToUpperInvariant();

        Validate(title, code);

        var titleKey = title!.ToUpperInvariant();

        var repository = _unitOfWork.Repository<PersonType>();

        var duplicates = await repository.GetAsync(x =>
            x.IsActive &&
            (x.Code.ToUpper() == code || x.Title.ToUpper() == titleKey));

        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                "نوع کاربر با این عنوان یا کد قبلاً ثبت شده است.");

        var entity = new PersonType
        {
            Title = title!,
            Code = code!,
            Description = dto.Description?.Trim(),
            IsActive = true
        };

        await repository.AddAsync(entity);
        await _unitOfWork.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<PersonTypeDto?> UpdateAsync(
        int id,
        UpdatePersonTypeDto dto)
    {
        var repository = _unitOfWork.Repository<PersonType>();
        var entity = await repository.GetByIdAsync(id);

        if (entity is not { IsActive: true })
            return null;

        var title = dto.Title?.Trim();
        var code = dto.Code?.Trim().ToUpperInvariant();

        Validate(title, code);

        var titleKey = title!.ToUpperInvariant();

        var duplicates = await repository.GetAsync(x =>
            x.IsActive &&
            x.Id != id &&
            (x.Code.ToUpper() == code || x.Title.ToUpper() == titleKey));

        if (duplicates.Count > 0)
            throw new InvalidOperationException(
                "نوع کاربر دیگری با این عنوان یا کد وجود دارد.");

        entity.Title = title!;
        entity.Code = code!;
        entity.Description = dto.Description?.Trim();

        // GetByIdAsync در ریپازیتوری تو از FindAsync استفاده می‌کند؛
        // بنابراین entity تحت نظارت DbContext است.
        await _unitOfWork.CompleteAsync();

        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Repository<PersonType>()
            .GetByIdAsync(id);

        if (entity is not { IsActive: true })
            return false;

        var persons = await _unitOfWork.Repository<Person>()
            .GetAsync(x => x.IsActive && x.PersonTypeId == id);

        if (persons.Count > 0)
            throw new InvalidOperationException(
                "این نوع کاربر به اشخاص فعال متصل است و قابل حذف نیست.");

        entity.IsActive = false;
        await _unitOfWork.CompleteAsync();

        return true;
    }

    private static void Validate(string? title, string? code)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان نوع کاربر الزامی است.");

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("کد نوع کاربر الزامی است.");
    }

    private static PersonTypeDto MapToDto(PersonType entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Code = entity.Code,
        Description = entity.Description,
        IsActive = entity.IsActive
    };
}
