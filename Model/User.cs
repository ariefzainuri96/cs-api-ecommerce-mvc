using System.ComponentModel.DataAnnotations.Schema;

namespace Ecommerce.Model;

[Table("users")]
public class User
{
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    [Column("password")]
    public string Password { get; set; } = string.Empty;
    [Column("is_admin")]
    public bool IsAdmin { get; set; }
}
