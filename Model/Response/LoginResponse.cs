namespace Ecommerce.Model.Response;

public class LoginResponse
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}
