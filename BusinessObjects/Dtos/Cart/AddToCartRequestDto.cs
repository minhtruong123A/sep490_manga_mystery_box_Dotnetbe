using System.ComponentModel;

namespace BusinessObjects.Dtos.Cart;

public class AddToCartRequestDto
{
    public string? SellProductId { get; set; }
    public string? MangaBoxId { get; set; }

    [DefaultValue(1)] public int? Quantity { get; set; }
}