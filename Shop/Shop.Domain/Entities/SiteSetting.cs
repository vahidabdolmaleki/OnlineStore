using Shop.Domain.Common;

namespace Shop.Domain.Entities
{
    public class SiteSetting : BaseEntity
    {
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string? Group { get; set; }

        public string? Description { get; set; }

        public string? DataType { get; set; }
    }
}