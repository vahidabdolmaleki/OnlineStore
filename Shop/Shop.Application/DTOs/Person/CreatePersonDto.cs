namespace Shop.Application.DTOs.Person;

public class CreatePersonDto
{
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NationalCode { get; set; }
    public int PersonTypeId { get; set; }
}
