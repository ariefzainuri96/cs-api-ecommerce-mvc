using Ecommerce.Model.Dto;
using Ecommerce.Model.Response;
using Ecommerce.Services.AuthService;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<BaseResponse<string>>> Login([FromBody] LoginDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Returns validation errors
        }

        var (result, user, token) = await authService.LoginAsync(request);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<LoginResponse>() { Status = 200, Message = "Success Login", Data = new LoginResponse() { Name = user.Name, Token = token, IsAdmin = user.IsAdmin } });
    }

    [HttpPost("register")]
    public async Task<ActionResult<BaseResponse<string>>> Register([FromBody] RegisterDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Returns validation errors
        }

        var error = await authService.RegisterAsync(request);

        if (error != null)
        {
            return error;
        }

        return Ok(new BaseResponse<string>() { Status = 200, Message = "Success Register" });
    }
}
