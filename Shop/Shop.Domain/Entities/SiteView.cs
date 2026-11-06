using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class SiteView : BaseEntity
    {
        public string PageUrl { get; set; } = string.Empty;

        public string? PageTitle { get; set; }

        public int? PersonId { get; set; }

        public string? IpAddress { get; set; }

        public string? UserAgent { get; set; }

        public string? Referrer { get; set; }

        public DateTime ViewedAtUtc { get; set; } = DateTime.UtcNow;

        public Person? Person { get; set; }
    }
}