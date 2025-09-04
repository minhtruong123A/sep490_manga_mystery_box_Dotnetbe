using System.ComponentModel.DataAnnotations;

namespace BusinessObjects.Dtos.Product;

public class SellProductCreateDto
{
    public string UserProductId { get; set; }
    public int Quantity { get; set; }

    [StringLength(300, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 300 characters.")]
    public string Description { get; set; }

    public int Price { get; set; }
}