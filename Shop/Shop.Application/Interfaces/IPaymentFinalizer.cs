using Shop.Domain.Enums;

namespace Shop.Application.Interfaces;

public interface IPaymentFinalizer
{
    Task FinalizeAsync(int orderId,PaymentMethod method,string referenceNumber,CancellationToken cancellationToken = default);
}
