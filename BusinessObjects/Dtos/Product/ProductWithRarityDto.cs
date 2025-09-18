namespace BusinessObjects.Dtos.Product;

public class ProductWithRarityDto
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public string UrlImage { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public int QuantityCurrent { get; set; }
    public int Status { get; set; }
    public string RarityName { get; set; }
    public DateTime CreateAt { get; set; }
}