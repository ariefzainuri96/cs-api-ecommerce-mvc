using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;

namespace Ecommerce.Services.CartService;

public interface ICartService
{
    Task<HttpError?> AddProductToCartAsync(int userId, AddProductCartDto dto);
    Task<(HttpError?, List<ShoppingCart>)> GetCartAsync(int userId);
}
