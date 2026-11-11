namespace Shop.Application.DTOs.Person;

public class UpdatePersonDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? NationalCode { get; set; }
}
