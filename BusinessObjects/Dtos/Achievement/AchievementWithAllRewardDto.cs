using BusinessObjects.Dtos.Reward;

namespace BusinessObjects.Dtos.Achievement;

public class AchievementWithAllRewardDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CollectionId { get; set; }
    public List<RewardGetDto> dtos { get; set; }
    public DateTime Create_at { get; set; }
}