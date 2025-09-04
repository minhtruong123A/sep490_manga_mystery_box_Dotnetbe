namespace BusinessObjects.Dtos.Reward;

public class ReawrdCompletionProgressOfUserCollectionDto
{
    public string AchievementId { get; set; }
    public int Conditions { get; set; }
    public string? Url_image { get; set; }
    public string MangaBoxId { get; set; }
    public string? MangaBox_image { get; set; }
    public int Quantity_box { get; set; }
    public bool isComplete { get; set; }
}