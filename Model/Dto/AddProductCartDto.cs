using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Model.Dto;

public class AddProductCartDto
{
    [Required(ErrorMessage = "Product Id is required")]
    public required int ProductId { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    public required int Quantity { get; set; }
}
