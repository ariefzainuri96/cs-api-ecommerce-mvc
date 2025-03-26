using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Ecommerce.Services.AuthService;

public class AuthService(EcommerceDbContext context, ILogger<AuthService> logger, IConfiguration configuration) : IAuthService
{
    public async Task<(HttpError?, User, string)> LoginAsync(LoginDto request)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return (new HttpError("Invalid Email or Password!") { StatusCode = StatusCodes.Status404NotFound }, new(), "");
            }

            var hashedPassword = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);

            if (hashedPassword == PasswordVerificationResult.Failed)
            {
                return (new HttpError("Invalid Email or Password!") { StatusCode = StatusCodes.Status404NotFound }, new(), "");
            }

            var token = CreateToken(user);

            return (null, user, token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in LoginAsync");
            return (new HttpError("Internal server Error!") { StatusCode = 500 }, new(), "");
        }
    }

    public async Task<HttpError?> RegisterAsync(RegisterDto request)
    {
        try
        {
            if (await context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return new HttpError("User Already Exist!") { StatusCode = StatusCodes.Status409Conflict };
            }

            var hashedPassword = new PasswordHasher<RegisterDto>().HashPassword(request, request.Password);

            await context.Users.AddAsync(new User { Name = request.Name, Email = request.Email, Password = hashedPassword, IsAdmin = false });
            await context.SaveChangesAsync();

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in RegisterAsync");
            return new HttpError("Internal server error") { StatusCode = 500 };
        }
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
