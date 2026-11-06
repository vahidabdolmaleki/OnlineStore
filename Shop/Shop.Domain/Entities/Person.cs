using Shop.Domain.Common;
using System.Net;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace Shop.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime? BirthDate { get; set; }

        public string? NationalCode { get; set; }

        public int PersonTypeId { get; set; }

        public int? RankId { get; set; }

        public int Score { get; set; } = 0;

        public PersonType PersonType { get; set; } = null!;
        

        public ICollection<Address> Addresses { get; set; }
            = new List<Address>();

        public ICollection<Order> Orders { get; set; }
            = new List<Order>();

        public ICollection<ShoppingBasket> ShoppingBaskets { get; set; }
            = new List<ShoppingBasket>();

        public ICollection<Images> Images { get; set; }
            = new List<Images>();

        public ICollection<Document> Documents { get; set; }
            = new List<Document>();

        public ICollection<ProductReview> Reviews { get; set; }
            = new List<ProductReview>();
    }
}