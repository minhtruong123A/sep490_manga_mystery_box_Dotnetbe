using BusinessObjects.Dtos.MangaBox;

namespace BusinessObjects.Dtos.Cart;

public class CartBoxDto
{
    public string CartBoxId { get; set; } = null!;
    public string MangaBoxId { get; set; } = null!;
    public MangaBoxDetailDto Box { get; set; } = null!;
    public int Quantity { get; set; }
}