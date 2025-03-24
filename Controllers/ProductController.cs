using System.Reflection;
using Ecommerce.Data;
using Ecommerce.Model;
using Ecommerce.Model.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(EcommerceDbContext context) : ControllerBase
{

    private readonly EcommerceDbContext _context = context;

    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<Product>>>> GetProducts()
    {
        return Ok(new BaseResponse<List<Product>>
        {
            Status = 200,
            Message = "Sukses",
            Data = await _context.Products
            .ToListAsync()
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<Product>>> GetProductById(int id)
    {
        Product? product = await _context.Products.FindAsync(id);

        if (product is null) return NotFound($"Product with specified ID: {id} Not Found!");

        return Ok(new BaseResponse<Product>
        {
            Status = 200,
            Message = "Sukses",
            Data = product
        });
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<Product>>> PostProduct(Product request)
    {
        await _context.Products.AddAsync(request);
        await _context.SaveChangesAsync();

        return Ok(new BaseResponse<Product>()
        {
            Status = 200,
            Message = $"Video game is Added!",
            Data = request
        });
    }

    //// Below is example when you want to automatically update the field based on the user request
    //// it is good when the model is big, but overkilled when the model is simple
    //// refer to [UpdateVideoGame] function to get simple example
    [HttpPatch("{id}")]
    public ActionResult<BaseResponse<Product>> PatchProduct(int id, [FromBody] Dictionary<string, object> updates)
    {
        if (updates == null || updates.Count == 0)
        {
            return BadRequest("No fields to update.");
        }

        // Find the video game by ID
        var product = _context.Products.FirstOrDefault(vg => vg.Id == id);

        if (product == null)
        {
            return NotFound($"VideoGame with ID {id} not found.");
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
                return BadRequest($"Invalid property: {propertyName}");
            }
        }

        _context.SaveChanges();

        return Ok(new BaseResponse<Product>()
        {
            Status = 200,
            Message = "Succesfully update the Video Game!",
            Data = product
        });
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteProduct(int id)
    {
        var product = _context.Products.FirstOrDefault(p => p.Id == id);

        if (product is null) return NotFound("Product that you want to delete is not found!");

        _context.Products.Remove(product);

        return Ok(new BaseResponse<string>() { Status = 200, Message = "Succesfully delete product!" });
    }
}
