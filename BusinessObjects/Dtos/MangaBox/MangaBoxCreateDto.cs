using Microsoft.AspNetCore.Http;

namespace BusinessObjects.Dtos.MangaBox;

public class MangaBoxCreateDto
{
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int Price { get; set; }
    public int TotalProduct { get; set; }
    public IFormFile? ImageUrl { get; set; }
    public string CollectionTopicId { get; set; }
    public int Quantity { get; set; }
    public DateTime Start_time { get; set; }
    public DateTime End_time { get; set; }
}