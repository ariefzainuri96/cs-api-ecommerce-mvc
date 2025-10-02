namespace Ecommerce.Model.Response;

public class ShoppingCartResponse
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string? ProductName { get; set; }
    public double? ProductPrice { get; set; }
    public int? ProductQuantity { get; set; }
    public int CartQuantity { get; set; }
}
