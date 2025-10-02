using System.Security.Claims;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Services.CartService;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BaseResponse<string>>> AddProductToCart([FromBody] AddProductCartDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string? userIdString = userIdClaim?.Value;
            int? userId = int.TryParse(userIdString, out int value) ? value : null;

            if (userId == null)
            {
                return new HttpError("Please relogin to get the correct Authorization") { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var result = await cartService.AddProductToCartAsync(userId.Value, dto);

            if (result != null)
            {
                return result;
            }

            return Ok(new BaseResponse<string>
            {
                Status = 200,
                Message = "Sukses",
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShoppingCart>>>> GetCart([FromQuery] PaginationRequestDto requestDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            string? userIdString = userIdClaim?.Value;
            int? userId = int.TryParse(userIdString, out int value) ? value : null;

            if (userId == null)
            {
                return new HttpError("Please relogin to get the correct Authorization") { StatusCode = StatusCodes.Status401Unauthorized };
            }

            var (result, response) = await cartService.GetCartAsync(userId.Value, requestDto);

            if (result != null)
            {
                return result;
            }

            return Ok(new BaseResponse<PaginationBaseResponse<ShoppingCartResponse>>
            {
                Status = 200,
                Message = "Sukses",
                Data = response
            });
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<string>>> DeleteProductFromCart(int id)
        {
            var error = await cartService.DeleteProductFromCartAsync(id);

            if (error != null)
            {
                return error;
            }

            return Ok(new BaseResponse<string> { Status = 200, Message = "Succesfully delete product from cart!" });
        }
    }
}
