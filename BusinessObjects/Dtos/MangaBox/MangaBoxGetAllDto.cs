namespace BusinessObjects.Dtos.MangaBox;

public class MangaBoxGetAllDto
{
    public string Id { get; set; }
    public string MysteryBoxName { get; set; }
    public int MysteryBoxPrice { get; set; }
    public string UrlImage { get; set; }
    public string CollectionTopic { get; set; }
    public DateTime? CreatedAt { get; set; } = null;
    public int Status { get; set; }
}