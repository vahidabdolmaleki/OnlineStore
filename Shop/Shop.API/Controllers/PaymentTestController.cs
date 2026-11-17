using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Shop.Application.Interfaces;
using Shop.Domain.Enums;

[ApiController]
[Route("api/dev/payments")]
public sealed class PaymentTestController(
    IPaymentFinalizer paymentFinalizer,
    IWebHostEnvironment environment) : ControllerBase
{
    [HttpPost("{orderId:int}/finalize")]
    public async Task<IActionResult> FinalizePayment(
        int orderId,
        [FromBody] FinalizePaymentTestRequest request,
        CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment())
            return NotFound();

        try
        {
            await paymentFinalizer.FinalizeAsync(
                orderId,
                PaymentMethod.Online,
                request.ReferenceNumber,
                cancellationToken);

            return Ok(new
            {
                message = "نهایی‌سازی تستی با موفقیت انجام شد.",
                orderId,
                referenceNumber = request.ReferenceNumber
            });
        }
        catch (ArgumentException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
        catch (InvalidOperationException exception)
        {
            return Conflict(new { message = exception.Message });
        }
    }
}

public sealed class FinalizePaymentTestRequest
{
    public string ReferenceNumber { get; set; } = string.Empty;
}
