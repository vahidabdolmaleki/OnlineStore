namespace Shop.Application.DTOs.Person
{
    public class CreatePersonTypeDto
    {
        public string Title { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
