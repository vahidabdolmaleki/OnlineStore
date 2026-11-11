using Shop.Application.DTOs.Addresses;
using Shop.Application.Interfaces;
using Shop.Domain.Entities;
using Shop.Domain.Interfaces;

namespace Shop.Application.Services;

public sealed class AddressService : IAddressService
{
    private readonly IUnitOfWork _unitOfWork;

    public AddressService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AddressDto>> GetAllAsync(int personId)
    {
        await EnsurePersonExistsAsync(personId);

        var addresses = await _unitOfWork.Repository<Address>()
            .GetAsync(x => x.PersonId == personId);

        if (addresses.Count == 0)
            return Array.Empty<AddressDto>();

        var detailIds = addresses.Select(x => x.AddressDetailsId).ToArray();

        var details = await _unitOfWork.Repository<AddressDetails>()
            .GetAsync(x => detailIds.Contains(x.Id));

        var detailsById = details.ToDictionary(x => x.Id);

        return addresses
            .OrderByDescending(x => x.IsDefault)
            .ThenBy(x => x.Id)
            .Select(address =>
            {
                if (!detailsById.TryGetValue(address.AddressDetailsId, out var detail))
                    throw new InvalidOperationException(
                        $"جزئیات آدرس {address.Id} یافت نشد.");

                return ToDto(address, detail);
            })
            .ToList();
    }

    public async Task<AddressDto> GetByIdAsync(int personId, int addressId)
    {
        var address = await GetOwnedAddressAsync(personId, addressId);
        var detail = await GetDetailAsync(address.AddressDetailsId);

        return ToDto(address, detail);
    }

    public async Task<AddressDto> CreateAsync(int personId, SaveAddressDto dto)
    {
        await EnsurePersonExistsAsync(personId);
        await ValidateAsync(dto);

        var existing = await _unitOfWork.Repository<Address>()
            .GetAsync(x => x.PersonId == personId);

        var detail = MakeDetail(dto);

        var address = new Address
        {
            PersonId = personId,
            Title = dto.Title.Trim(),
            ReceiverName = dto.ReceiverName,
            ReceiverPhone = dto.ReceiverPhone,
            IsDefault = existing.Count == 0,
            AddressDetails = detail
        };

        await _unitOfWork.Repository<Address>().AddAsync(address);
        await _unitOfWork.CompleteAsync();

        return ToDto(address, detail);
    }

    public async Task<AddressDto> UpdateAsync(
        int personId, int addressId, SaveAddressDto dto)
    {
        var address = await GetOwnedAddressAsync(personId, addressId);
        await ValidateAsync(dto);

        // جزئیات جدید می‌سازیم تا تغییر آدرس، داده احتمالیِ مشترک
        // یا نشانی ثبت‌شده در سوابق قبلی را دست‌کاری نکند.
        var newDetail = MakeDetail(dto);

        await _unitOfWork.Repository<AddressDetails>()
            .AddAsync(newDetail);

        address.Title = dto.Title.Trim();
        address.ReceiverName = dto.ReceiverName;
        address.ReceiverPhone = dto.ReceiverPhone;
        address.AddressDetails = newDetail;
        address.EditDateUtc = DateTime.UtcNow;

        _unitOfWork.Repository<Address>().Update(address);
        await _unitOfWork.CompleteAsync();

        return ToDto(address, newDetail);
    }

    public async Task<AddressDto> SetDefaultAsync(int personId, int addressId)
    {
        await EnsurePersonExistsAsync(personId);

        var addresses = await _unitOfWork.Repository<Address>()
            .GetAsync(x => x.PersonId == personId);

        var target = addresses.FirstOrDefault(x => x.Id == addressId)
            ?? throw new KeyNotFoundException("آدرس یافت نشد.");

        if (!target.IsDefault)
        {
            foreach (var address in addresses.Where(x => x.IsDefault))
            {
                address.IsDefault = false;
                address.EditDateUtc = DateTime.UtcNow;
                _unitOfWork.Repository<Address>().Update(address);
            }

            target.IsDefault = true;
            target.EditDateUtc = DateTime.UtcNow;
            _unitOfWork.Repository<Address>().Update(target);

            await _unitOfWork.CompleteAsync();
        }

        var detail = await GetDetailAsync(target.AddressDetailsId);
        return ToDto(target, detail);
    }

    public async Task DeleteAsync(int personId, int addressId)
    {
        var address = await GetOwnedAddressAsync(personId, addressId);

        if (address.IsDefault)
        {
            var others = await _unitOfWork.Repository<Address>()
                .GetAsync(x => x.PersonId == personId && x.Id != addressId);

            var nextDefault = others.OrderBy(x => x.Id).FirstOrDefault();

            if (nextDefault is not null)
            {
                nextDefault.IsDefault = true;
                nextDefault.EditDateUtc = DateTime.UtcNow;
                _unitOfWork.Repository<Address>().Update(nextDefault);
            }
        }

        address.IsDefault = false;
        address.EditDateUtc = DateTime.UtcNow;

        // با پیاده‌سازی فعلی ShopDbContext، Delete هنگام ذخیره
        // به IsActive = false تبدیل می‌شود.
        _unitOfWork.Repository<Address>().Delete(address);
        await _unitOfWork.CompleteAsync();
    }

    private async Task EnsurePersonExistsAsync(int personId)
    {
        if (personId < 1 ||
            await _unitOfWork.Repository<Person>().GetByIdAsync(personId) is null)
        {
            throw new KeyNotFoundException("شخص فعال یافت نشد.");
        }
    }

    private async Task<Address> GetOwnedAddressAsync(
        int personId, int addressId)
    {
        await EnsurePersonExistsAsync(personId);

        var address = await _unitOfWork.Repository<Address>()
            .GetByIdAsync(addressId);

        if (address is null || address.PersonId != personId)
            throw new KeyNotFoundException("آدرس یافت نشد.");

        return address;
    }

    private async Task<AddressDetails> GetDetailAsync(int detailId)
    {
        return await _unitOfWork.Repository<AddressDetails>()
            .GetByIdAsync(detailId)
            ?? throw new KeyNotFoundException("جزئیات آدرس یافت نشد.");
    }

    private async Task ValidateAsync(SaveAddressDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) ||
            string.IsNullOrWhiteSpace(dto.Street))
        {
            throw new ArgumentException("عنوان و خیابان الزامی هستند.");
        }

        if (dto.Latitude is < -90 or > 90 ||
            dto.Longitude is < -180 or > 180)
        {
            throw new ArgumentException("مختصات جغرافیایی معتبر نیست.");
        }

        if (dto.StateCityId < 1 ||
            await _unitOfWork.Repository<StateCity>()
                .GetByIdAsync(dto.StateCityId) is null)
        {
            throw new ArgumentException("شهر/منطقه فعال یافت نشد.");
        }
    }

    private static AddressDetails MakeDetail(SaveAddressDto dto) => new()
    {
        StateCityId = dto.StateCityId,
        Street = dto.Street.Trim(),
        Alley = dto.Alley,
        Number = dto.Number,
        Unit = dto.Unit,
        PostalCode = dto.PostalCode,
        Description = dto.Description,
        Latitude = dto.Latitude,
        Longitude = dto.Longitude
    };

    private static AddressDto ToDto(Address address, AddressDetails detail) =>
        new(
            address.Id,
            address.PersonId,
            detail.Id,
            address.Title,
            address.ReceiverName,
            address.ReceiverPhone,
            address.IsDefault,
            detail.StateCityId,
            detail.Street,
            detail.Alley,
            detail.Number,
            detail.Unit,
            detail.PostalCode,
            detail.Description,
            detail.Latitude,
            detail.Longitude
        );
}
