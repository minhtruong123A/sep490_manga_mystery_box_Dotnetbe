using BusinessObjects.Dtos.Reward;

namespace BusinessObjects.Dtos.Achievement;

public class AchievementOfUserCollectionCompletionProgressDto
{
    public string Id { get; set; }
    public string AchievementName { get; set; }
    public string CollectionId { get; set; }
    public string CollectionName { get; set; }
    public int Count { get; set; }
    public List<ReawrdCompletionProgressOfUserCollectionDto> dtos { get; set; }
}