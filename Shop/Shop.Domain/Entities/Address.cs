using Shop.Domain.Common;
using Shop.Domain.Entities;

public class Address : BaseEntity
    {
        public int PersonId { get; set; }

        public int AddressDetailsId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? ReceiverName { get; set; }

        public string? ReceiverPhone { get; set; }

        public bool IsDefault { get; set; }

        public Person Person { get; set; } = null!;

        public AddressDetails AddressDetails { get; set; } = null!;

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();
}
