using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Services.ProductService;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(IProductService productService, IValidator<AddProductDto> validator) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<Product>>>> GetProducts([FromQuery] PaginationRequestDto requestDto)
    {
        var (result, products) = await productService.GetProductsAsync(requestDto);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<PaginationBaseResponse<Product>>
        {
            Status = 200,
            Message = "Sukses",
            Data = products
        });
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<Product>>> GetProductById(int id)
    {
        var (result, product) = await productService.GetProductByIdAsync(id);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<Product>
        {
            Status = 200,
            Message = "Sukses",
            Data = product
        });
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<BaseResponse<Product>>> PostProduct([FromBody] AddProductDto request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var (result, product) = await productService.PostProductAsync(request);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<Product>()
        {
            Status = 200,
            Message = $"Video game is Added!",
            Data = product
        });
    }

    //// Below is example when you want to automatically update the field based on the user request
    //// it is good when the model is big, but overkilled when the model is simple
    //// refer to [UpdateVideoGame] function to get simple example
    [Authorize]
    [HttpPatch("{id}")]
    public async Task<ActionResult<BaseResponse<Product>>> PatchProduct(int id, [FromBody] Dictionary<string, object> updates)
    {
        if (updates == null || updates.Count == 0)
        {
            return BadRequest("No fields to update.");
        }

        var (result, product) = await productService.PatchProductAsync(id, updates);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<Product>()
        {
            Status = 200,
            Message = "Succesfully update the Video Game!",
            Data = product
        });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(int id)
    {
        var error = await productService.DeleteProductAsync(id);

        if (error != null)
        {
            return error;
        }

        return Ok(new BaseResponse<string>() { Status = 200, Message = "Succesfully delete product!" });
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult> PutProduct(int id, [FromBody] ProductDto productDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (error, product) = await productService.PutProductAsync(id, productDto);

        if (error != null)
        {
            return error;
        }

        return Ok(new BaseResponse<Product>()
        {
            Status = 200,
            Message = "Succesfully update the Product!",
            Data = product
        });
    }
}
