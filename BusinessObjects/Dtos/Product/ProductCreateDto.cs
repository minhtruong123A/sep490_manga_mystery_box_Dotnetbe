using Microsoft.AspNetCore.Http;

namespace BusinessObjects.Dtos.Product;

public class ProductCreateDto
{
    public string Name { get; set; }
    public string RarityName { get; set; }
    public string Description { get; set; }
    public IFormFile? UrlImage { get; set; }
    public string CollectionId { get; set; }
}