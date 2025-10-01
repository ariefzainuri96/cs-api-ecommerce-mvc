using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
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

    public async Task<(HttpError?, List<ShoppingCart>)> GetCartAsync(int userId)
    {
        try
        {
            var carts = await context.ShoppingCarts.Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .Include(c => c.User)
            .ToListAsync();

            return (null, carts);
        }
        catch (System.Exception)
        {

            return (new HttpError("Internal server error") { StatusCode = 500 }, []);
        }
    }
}
