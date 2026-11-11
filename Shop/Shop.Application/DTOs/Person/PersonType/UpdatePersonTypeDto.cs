namespace Shop.Application.DTOs.Person.PersonType
{
    public class UpdatePersonTypeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
