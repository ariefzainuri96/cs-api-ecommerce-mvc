using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model;

[Table("products")]
public class Product
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("description")]
    public string Description { get; set; } = string.Empty;
    [Column("price")]
    public int Price { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
