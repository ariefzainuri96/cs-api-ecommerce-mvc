using System.Reflection;
using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.ProductService;

public class ProductService(EcommerceDbContext context, ILogger<ProductService> logger) : IProductService
{
    public async Task<HttpError?> DeleteProductAsync(int id)
    {
        try
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
            {
                return new HttpError("Product that you want to delete is not found!") { StatusCode = StatusCodes.Status404NotFound };
            }

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            return null;
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in DeleteProductAsync");
            return new HttpError("Internal server error") { StatusCode = 500 };
        }
    }

    public async Task<(HttpError?, Product)> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await context.Products.FindAsync(id);

            if (product is null)
            {
                return (new HttpError("Product with specified ID not found!") { StatusCode = StatusCodes.Status404NotFound }, new());
            }

            return (null, product);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in GetProductByIdAsync");
            return (new HttpError("Internal server error") { StatusCode = 500 }, new());
        }
    }

    public async Task<(HttpError?, List<Product>)> GetProductsAsync()
    {
        try
        {
            var products = await context.Products.ToListAsync();

            return (null, products);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in GetProductsAsync");
            return (new HttpError("Internal server error") { StatusCode = 500 }, []);
        }
    }

    public async Task<(HttpError?, Product)> PatchProductAsync(int id, Dictionary<string, object> updates)
    {
        try
        {
            // Find the video game by ID
            var product = await context.Products.FirstOrDefaultAsync(vg => vg.Id == id);

            if (product == null)
            {
                return (new HttpError($"VideoGame with ID {id} not found.") { StatusCode = StatusCodes.Status404NotFound }, new());
            }

            // Update fields dynamically
            foreach (var update in updates)
            {
                var propertyName = update.Key;
                var propertyValue = update.Value;

                // Use reflection to check if the property exists and is writable
                var propertyInfo = typeof(Product).GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (propertyInfo != null && propertyInfo.CanWrite)
                {
                    // Convert the value to the correct type and set it
                    propertyInfo.SetValue(product, Convert.ChangeType(propertyValue, propertyInfo.PropertyType));
                }
                else
                {
                    return (new HttpError($"Invalid property: {propertyName}") { StatusCode = StatusCodes.Status400BadRequest }, new());
                }
            }

            context.SaveChanges();

            return (null, product);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in PatchProductAsync");
            return (new HttpError("Internal server error") { StatusCode = 500 }, new());
        }
    }

    public async Task<(HttpError?, Product)> PostProductAsync(AddProductDto request)
    {
        try
        {
            var product = new Product { Name = request.Name, Description = request.Description, Price = request.Price, Quantity = request.Quantity };
            await context.Products.AddAsync(product);

            await context.SaveChangesAsync();

            return (null, product);
        }
        catch (System.Exception ex)
        {
            logger.LogError(ex, "Error in PatchProductAsync");
            return (new HttpError("Internal server error") { StatusCode = 500 }, new());
        }
    }
}
