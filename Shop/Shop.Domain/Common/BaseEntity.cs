namespace Shop.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }

        public DateTime CreateDateUtc { get; set; } = DateTime.UtcNow;

        public DateTime? EditDateUtc { get; set; }

        public int? CreatedByPersonId { get; set; }

        public int? ModifiedByPersonId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}