namespace BusinessObjects.Dtos.MangaBox;

public class MangaBoxDto
{
    public string MysteryBoxId { get; set; }
    public string CollectionTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public int Status { get; set; }
}