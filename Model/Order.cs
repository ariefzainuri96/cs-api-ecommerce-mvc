using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model;

[Table("orders")]
public class Order
{
    [Column("id")]
    public int Id { get; set; }
    [Column("user_id")]
    public int UserId { get; set; }
    [Column("total_price")]
    public int TotalPrice { get; set; }
    [Column("status")]
    public string Status { get; set; } = string.Empty;
    [Column("created_at")]
    public string CreatedAt { get; set; } = string.Empty;
    [Column("updated_at")]
    public string UpdatedAt { get; set; } = string.Empty;
    [Column("product_id")]
    public int ProductId { get; set; }
    [Column("quantity")]
    public int Quantity { get; set; }
    [Column("invoice_id")]
    public string InvoiceId { get; set; } = string.Empty;
    [Column("invoice_url")]
    public string InvoiceUrl { get; set; } = string.Empty;
    [Column("invoice_exp_date")]
    public string InvoiceExpDate { get; set; } = string.Empty;
}
