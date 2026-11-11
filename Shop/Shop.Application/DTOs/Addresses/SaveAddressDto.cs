using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.Addresses;

public sealed class SaveAddressDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }

    [Range(1, int.MaxValue)]
    public int StateCityId { get; set; }

    [Required]
    public string Street { get; set; } = string.Empty;

    public string? Alley { get; set; }
    public string? Number { get; set; }
    public string? Unit { get; set; }
    public string? PostalCode { get; set; }
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
