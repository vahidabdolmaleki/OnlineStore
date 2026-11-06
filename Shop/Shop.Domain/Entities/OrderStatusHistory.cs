using Shop.Domain.Common;
using Shop.Domain.Entities;
using Shop.Domain.Enums;

public class OrderStatusHistory : BaseEntity
    {
        public int OrderId { get; set; }

        public OrderStatus Status { get; set; }

        public string? Description { get; set; }

        public int? ChangedByPersonId { get; set; }

        public Order Order { get; set; } = null!;
    }
