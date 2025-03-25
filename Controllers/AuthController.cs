using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Model.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(EcommerceDbContext context, IConfiguration configuration) : ControllerBase
{
    EcommerceDbContext _context = context;

    [HttpPost("login")]
    public ActionResult<BaseResponse<string>> Login(LoginDto request)
    {
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return NotFound("Invalid Email or Password!");
        }

        var hashedPassword = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);

        if (hashedPassword == PasswordVerificationResult.Failed)
        {
            return NotFound("Invalid Email or Password!");
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

        var hashedPassword = new PasswordHasher<RegisterDto>().HashPassword(request, request.Password);

        await _context.Users.AddAsync(new User { Name = request.Name, Email = request.Email, Password = hashedPassword, IsAdmin = false });
        var result = await _context.SaveChangesAsync();

        if (result == 0)
        {
            return BadRequest("Failed to register");
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
