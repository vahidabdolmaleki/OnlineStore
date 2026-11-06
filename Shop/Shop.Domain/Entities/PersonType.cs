using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class PersonType : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public string Code { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<Person> Persons { get; set; }
            = new List<Person>();
    }
}