using Ecommerce.Data;
using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Services;

public class AuthService(EcommerceDbContext context, IConfiguration configuration) : IAuthService
{
    public (ServiceError?, User) LoginAsync(LoginDto request)
    {
        var user = context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null)
        {
            return (new ServiceError("Invalid Email or Password!") { StatusCode = StatusCodes.Status404NotFound }, new());
        }

        var hashedPassword = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, request.Password);

        if (hashedPassword == PasswordVerificationResult.Failed)
        {
            return (new ServiceError("Invalid Email or Password!") { StatusCode = StatusCodes.Status404NotFound }, new());
        }

        return (null, user);
    }

    public async Task<ServiceError?> RegisterAsync(RegisterDto request)
    {
        if (await context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return new ServiceError("User Already Exist!") { StatusCode = StatusCodes.Status409Conflict };
        }

        var hashedPassword = new PasswordHasher<RegisterDto>().HashPassword(request, request.Password);

        await context.Users.AddAsync(new User { Name = request.Name, Email = request.Email, Password = hashedPassword, IsAdmin = false });
        var result = await context.SaveChangesAsync();

        if (result == 0)
        {
            return new ServiceError("Internal server error") { StatusCode = 500 };
        }

        return null;
    }
}
