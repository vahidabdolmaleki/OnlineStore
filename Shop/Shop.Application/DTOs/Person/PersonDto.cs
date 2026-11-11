namespace Shop.Application.DTOs.Person;

public class PersonDto
{
    public int Id { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int PersonTypeId { get; set; }
    public string PersonTypeTitle { get; set; } = string.Empty;
    public int Score { get; set; }
}
