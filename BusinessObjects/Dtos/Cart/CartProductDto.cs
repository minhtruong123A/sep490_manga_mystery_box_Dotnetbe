using BusinessObjects.Dtos.Product;

namespace BusinessObjects.Dtos.Cart;

public class CartProductDto
{
    public string CartProductId { get; set; } = null!;
    public string SellProductId { get; set; } = null!;
    public SellProductDetailDto Product { get; set; } = null!;
    public int Quantity { get; set; }
}