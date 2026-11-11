using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.ShoppingBasket;
using Shop.Application.Interfaces;

namespace Shop.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShoppingBasketsController(IShoppingBasketService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBasket([FromQuery] int? personId, [FromQuery] string? guestToken)
    {
        var basket = await service.GetBasketAsync(personId, guestToken);
        return basket is null ? NotFound(new { message = "سبد خرید یافت نشد یا خالی است." }) : Ok(basket);
    }

    [HttpPost("items")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddItem([FromBody] AddItemToBasketDto dto)
    {
        try
        {
            var basket = await service.AddItemAsync(dto);
            return Ok(basket);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("items/{itemId:int}")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateQuantity(int itemId, [FromBody] UpdateBasketItemQuantityDto dto)
    {
        try
        {
            var basket = await service.UpdateItemQuantityAsync(itemId, dto);
            return Ok(basket);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("items/{itemId:int}")]
    [ProducesResponseType(typeof(BasketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveItem(int itemId)
    {
        try
        {
            var basket = await service.RemoveItemAsync(itemId);
            return Ok(basket);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{basketId:int}/clear")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearBasket(int basketId)
    {
        var cleared = await service.ClearBasketAsync(basketId);
        return cleared ? NoContent() : NotFound(new { message = "سبد خرید یافت نشد." });
    }
}
