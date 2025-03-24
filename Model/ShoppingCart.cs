using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model;

[Table("shopping_carts")]
public class ShoppingCart
{
    [Column("id")]
    public int Id { get; set; }
    [Column("product_id")]
    public int ProductId { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
    [Column("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
}
