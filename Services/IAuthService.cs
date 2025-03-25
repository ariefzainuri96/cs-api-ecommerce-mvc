using Ecommerce.Model.Dto;
using Ecommerce.Model.Entities;
using Ecommerce.Utils;

namespace Ecommerce.Services;

public interface IAuthService
{
    Task<ServiceError?> RegisterAsync(RegisterDto request);
    (ServiceError?, User) LoginAsync(LoginDto request);
}
