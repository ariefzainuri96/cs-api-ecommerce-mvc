using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IAuthService authService, IConfiguration configuration) : ControllerBase
{
    [HttpPost("login")]
    public ActionResult<BaseResponse<string>> Login(LoginDto request)
    {
        var (result, user) = authService.LoginAsync(request);

        if (result != null)
        {
            return result;
        }

        return Ok(new BaseResponse<LoginResponse>() { Status = 200, Message = "Success Login", Data = new LoginResponse() { Name = user.Name, Token = CreateToken(user), IsAdmin = user.IsAdmin } });
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

    private string CreateToken(User user)
    {
        var claims = new List<Claim>{
            new("name", user.Name),
            new("email", user.Email),
            new("is_admin", user.IsAdmin.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("AppSettings:Token")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: configuration.GetValue<string>("AppSettings:Issuer")!,
            audience: configuration.GetValue<string>("AppSettings:Audience")!,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
}
