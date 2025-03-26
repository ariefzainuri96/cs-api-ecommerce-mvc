using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;

namespace Ecommerce.Services.AuthService;

public interface IAuthService
{
    Task<HttpError?> RegisterAsync(RegisterDto request);
    Task<(HttpError?, User, string)> LoginAsync(LoginDto request);
}
