using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.Addresses;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/persons/{personId:int}/addresses")]
public sealed class AddressesController : ControllerBase
{
    private readonly IAddressService _addresses;

    public AddressesController(IAddressService addresses)
    {
        _addresses = addresses;
    }

    [HttpGet]
    public Task<IActionResult> GetAll(int personId) =>
        Run(async () => Ok(await _addresses.GetAllAsync(personId)));

    [HttpGet("{addressId:int}")]
    public Task<IActionResult> GetById(int personId, int addressId) =>
        Run(async () =>
            Ok(await _addresses.GetByIdAsync(personId, addressId)));

    [HttpPost]
    public Task<IActionResult> Create(
        int personId, [FromBody] SaveAddressDto dto) =>
        Run(async () =>
        {
            var address = await _addresses.CreateAsync(personId, dto);

            return CreatedAtAction(
                nameof(GetById),
                new { personId, addressId = address.Id },
                address);
        });

    [HttpPut("{addressId:int}")]
    public Task<IActionResult> Update(
        int personId, int addressId, [FromBody] SaveAddressDto dto) =>
        Run(async () =>
            Ok(await _addresses.UpdateAsync(personId, addressId, dto)));

    [HttpPut("{addressId:int}/default")]
    public Task<IActionResult> SetDefault(int personId, int addressId) =>
        Run(async () =>
            Ok(await _addresses.SetDefaultAsync(personId, addressId)));

    [HttpDelete("{addressId:int}")]
    public Task<IActionResult> Delete(int personId, int addressId) =>
        Run(async () =>
        {
            await _addresses.DeleteAsync(personId, addressId);
            return NoContent();
        });

    private async Task<IActionResult> Run(Func<Task<IActionResult>> action)
    {
        try
        {
            return await action();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
