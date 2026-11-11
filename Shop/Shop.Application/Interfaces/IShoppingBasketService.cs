using Shop.Application.DTOs.ShoppingBasket;

namespace Shop.Application.Interfaces;

public interface IShoppingBasketService
{
    Task<BasketDto?> GetBasketAsync(int? personId, string? guestToken);
    Task<BasketDto> AddItemAsync(AddItemToBasketDto dto);
    Task<BasketDto> UpdateItemQuantityAsync(int itemId, UpdateBasketItemQuantityDto dto);
    Task<BasketDto> RemoveItemAsync(int itemId);
    Task<bool> ClearBasketAsync(int basketId);
}
