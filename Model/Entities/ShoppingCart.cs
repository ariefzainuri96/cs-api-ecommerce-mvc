using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model.Entities;

[Table("shopping_carts")]
public class ShoppingCart
{
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    public Product? Product { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }
    public User? User { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
