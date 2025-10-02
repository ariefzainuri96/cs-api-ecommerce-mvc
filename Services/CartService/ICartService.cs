using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Utils;

namespace Ecommerce.Services.CartService;

public interface ICartService
{
    Task<HttpError?> AddProductToCartAsync(int userId, AddProductCartDto dto);
    Task<(HttpError?, PaginationBaseResponse<ShoppingCartResponse>)> GetCartAsync(int userId, PaginationRequestDto requestDto);
    Task<HttpError?> DeleteProductFromCartAsync(int cartId);
}
