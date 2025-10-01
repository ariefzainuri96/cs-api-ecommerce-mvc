using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model.Entities;

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
    public double Price { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}
