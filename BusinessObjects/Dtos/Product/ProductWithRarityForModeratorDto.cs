namespace BusinessObjects.Dtos.Product;

public class ProductWithRarityForModeratorDto
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string UrlImage { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public int QuantityCurrent { get; set; }
    public int Status { get; set; }
    public string RarityName { get; set; }
    public string CollectionId { get; set; }
    public bool is_Block { get; set; }
    public DateTime CreateAt { get; set; }
}