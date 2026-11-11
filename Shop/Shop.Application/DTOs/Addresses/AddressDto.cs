namespace Shop.Application.DTOs.Addresses;

public sealed record AddressDto(
    int Id,
    int PersonId,
    int AddressDetailsId,
    string Title,
    string? ReceiverName,
    string? ReceiverPhone,
    bool IsDefault,
    int StateCityId,
    string Street,
    string? Alley,
    string? Number,
    string? Unit,
    string? PostalCode,
    string? Description,
    decimal? Latitude,
    decimal? Longitude
);
