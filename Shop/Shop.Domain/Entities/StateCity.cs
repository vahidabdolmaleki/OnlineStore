using Shop.Domain.Common;
using Shop.Domain.Enums;

namespace Shop.Domain.Entities
{
    public class StateCity : BaseEntity
    {
        public string Title { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public StateCityType Type { get; set; }

        public StateCity? Parent { get; set; }

        public ICollection<StateCity> Children { get; set; }
            = new List<StateCity>();

        public ICollection<AddressDetails> Addresses { get; set; }
            = new List<AddressDetails>();
    }
}
