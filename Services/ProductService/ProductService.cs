using AutoMapper;
using Ecommerce.Data;
using Ecommerce.GlobalException;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Query;
using Ecommerce.Utils;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services.ProductService;

public class ProductService(EcommerceDbContext context, IMapper mapper) : IProductService
{
    public async Task<HttpError?> DeleteProductAsync(int id)
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

    public async Task<(HttpError?, Product)> GetProductByIdAsync(int id)
    {
        var product = await context.Products.FindAsync(id);

        if (product is null)
        {
            return (new HttpError("Product with specified ID not found!") { StatusCode = StatusCodes.Status404NotFound }, new());
        }

        return (null, product);
    }

    public async Task<(HttpError?, PaginationBaseResponse<Product>)> GetProductsAsync(PaginationRequestDto requestDto)
    {
        IQueryable<Product> query = ProductQuery.GetQuery(context, requestDto);

        // Calculate the total number of items BEFORE applying skip/take.
        int totalCount = await query.CountAsync();

        // Apply pagination logic (Skip and Take)
        List<Product> items = [.. await query
                .Skip((requestDto.Page - 1) * requestDto.PageSize) // Skip previous pages
                .Take(requestDto.PageSize) // Take only the requested page size
                .ToListAsync()];

        return (null, new PaginationBaseResponse<Product>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = requestDto.Page,
            PageSize = requestDto.PageSize,
        });
    }

    public async Task<(HttpError?, Product)> PatchProductAsync(int id, Dictionary<string, object> updates)
    {
        var product = await context.Products.FirstOrDefaultAsync(vg => vg.Id == id) ?? throw new EntityNotFoundException($"VideoGame with ID {id} not found.");

        var invalidPropertyList = EntityUtil.CheckEntityField<Product>(updates);
        if (invalidPropertyList.Count > 0)
        {
            throw new InvalidPropertyException($"Invalid property: {string.Join(", ", invalidPropertyList)}");
        }

        // Update fields dynamically
        EntityUtil.PatchEntity(product, updates);

        context.Entry(product).State = EntityState.Modified;

        context.SaveChanges();

        return (null, product);
    }

    public async Task<(HttpError?, Product)> PostProductAsync(AddProductDto request)
    {
        var product = new Product { Name = request.Name, Description = request.Description, Price = request.Price, Quantity = request.Quantity };
        await context.Products.AddAsync(product);

        await context.SaveChangesAsync();

        return (null, product);
    }

    public async Task<(HttpError?, Product)> PutProductAsync(int id, ProductDto product)
    {
        var existingProduct = await context.Products
            .FirstOrDefaultAsync(p => p.Id == id)
            ?? throw new EntityNotFoundException($"Product with ID {id} not found.");

        mapper.Map(product, existingProduct);

        context.Entry(existingProduct).State = EntityState.Modified;

        await context.SaveChangesAsync();

        return (null, existingProduct);
    }
}
