namespace Shop.Domain.Enums
{
    public enum StateCityType
    {
        Country = 1,
        Province = 2,
        City = 3,
        District = 4
    }
}
namespace Shop.Domain.Enums
{
    public enum ProductRelationType
    {
        Related = 1,
        Similar = 2,
        Complementary = 3,
        Replacement = 4
    }
}
namespace Shop.Domain.Enums
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Processing = 3,
        ReadyToShip = 4,
        Shipped = 5,
        Delivered = 6,
        Cancelled = 7,
        Returned = 8,
        Refunded = 9
    }
}
namespace Shop.Domain.Enums
{
    public enum PaymentStatus
    {
        Unpaid = 1,
        Pending = 2,
        Paid = 3,
        Failed = 4,
        Refunded = 5,
        PartiallyRefunded = 6
    }
}
namespace Shop.Domain.Enums
{
    public enum PaymentMethod
    {
        Online = 1,
        CashOnDelivery = 2,
        BankTransfer = 3,
        Wallet = 4
    }
}
namespace Shop.Domain.Enums
{
    public enum InventoryTransactionType
    {
        Purchase = 1,
        Sale = 2,
        Return = 3,
        Adjustment = 4,
        TransferIn = 5,
        TransferOut = 6,
        Reservation = 7,
        ReservationRelease = 8
    }
}
namespace Shop.Domain.Enums
{
    public enum ShippingStatus
    {
        Pending = 1,
        Preparing = 2,
        Shipped = 3,
        InTransit = 4,
        Delivered = 5,
        Failed = 6,
        Returned = 7
    }
}
namespace Shop.Domain.Enums
{
    public enum DocumentType
    {
        Invoice = 1,
        Warranty = 2,
        CustomerDocument = 3,
        OrderDocument = 4,
        Other = 5
    }
}
namespace Shop.Domain.Enums
{
    public enum CouponType
    {
        Percentage = 1,
        FixedAmount = 2
    }
}
namespace Shop.Domain.Enums
{
    public enum ReviewStatus
    {
        Pending = 1,
        Approved = 2,
        Rejected = 3
    }
}
namespace Shop.Domain.Enums
{
    public enum ArticleStatus
    {
        Draft = 1,

        PendingReview = 2,

        Published = 3,

        Scheduled = 4,

        Archived = 5
    }
}