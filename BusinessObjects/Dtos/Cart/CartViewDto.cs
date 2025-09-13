namespace BusinessObjects.Dtos.Cart;

public class CartViewDto
{
    public List<CartProductDto> Products { get; set; } = new();
    public List<CartBoxDto> Boxes { get; set; } = new();
}