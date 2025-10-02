using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Query;
using Ecommerce.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.CartService;

public class CartService(EcommerceDbContext context, ILogger<CartService> logger) : ICartService
{
    public async Task<HttpError?> AddProductToCartAsync(int userId, AddProductCartDto dto)
    {
        try
        {
            await context.ShoppingCarts.AddAsync(new ShoppingCart { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity });

            await context.SaveChangesAsync();

            return null;
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in PatchProductAsync");
            return new HttpError("Internal server error") { StatusCode = 500 };
        }
    }

    public async Task<HttpError?> DeleteProductFromCartAsync(int cartId)
    {
        try
        {
            await context.ShoppingCarts.Where(c => c.Id == cartId).ExecuteDeleteAsync();

            return null;
        }
        catch (System.Exception)
        {
            logger.LogError("Error in DeleteProductFromCartAsync");
            return new HttpError("Internal server error") { StatusCode = 500 };
        }
    }

    public async Task<(HttpError?, PaginationBaseResponse<ShoppingCartResponse>)> GetCartAsync(int userId, PaginationRequestDto requestDto)
    {
        try
        {
            IQueryable<ShoppingCart> query = ShoppingCartQuery.GetQuery(context, requestDto, userId);

            // Calculate the total number of items BEFORE applying skip/take.
            int totalCount = await query.CountAsync();

            // Apply pagination logic (Skip and Take)
            List<ShoppingCartResponse> items = [.. (await query
                .Skip((requestDto.Page - 1) * requestDto.PageSize) // Skip previous pages
                .Take(requestDto.PageSize) // Take only the requested page size
                .ToListAsync()).Select(cart => new ShoppingCartResponse
            {
                Id = cart.Id,
                ProductId = cart.Product?.Id,
                ProductName = cart.Product?.Name,
                ProductQuantity = cart.Product?.Quantity,
                ProductPrice = cart.Product?.Price,
                CartQuantity = cart.Quantity,
            })];


            return (null, new PaginationBaseResponse<ShoppingCartResponse>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = requestDto.Page,
                PageSize = requestDto.PageSize,
            });
        }
        catch (System.Exception)
        {

            return (new HttpError("Internal server error") { StatusCode = 500 }, new());
        }
    }
}
