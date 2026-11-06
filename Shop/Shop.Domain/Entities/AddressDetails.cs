using Shop.Domain.Common;
using Shop.Domain.Entities;

public class AddressDetails : BaseEntity
    {
        public int StateCityId { get; set; }

        public string Street { get; set; } = string.Empty;

        public string? Alley { get; set; }

        public string? Number { get; set; }

        public string? Unit { get; set; }

        public string? PostalCode { get; set; }

        public string? Description { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public StateCity StateCity { get; set; } = null!;

        public ICollection<Address> Addresses { get; set; }
            = new List<Address>();
    }
