using BusinessObjects.Dtos.Product;

namespace BusinessObjects.Dtos.MangaBox;

public class MangaBoxDetailDto
{
    public string Id { get; set; }
    public int Status { get; set; }
    public string MysteryBoxName { get; set; }
    public string MysteryBoxDescription { get; set; }
    public int MysteryBoxPrice { get; set; }
    public string CollectionTopic { get; set; }
    public string UrlImage { get; set; }
    public int Quantity { get; set; }
    public DateTime Create_at { get; set; }
    public DateTime Start_time { get; set; }
    public DateTime End_time { get; set; }
    public int TotalProduct { get; set; }
    public List<ProductInBoxDto> Products { get; set; }
}