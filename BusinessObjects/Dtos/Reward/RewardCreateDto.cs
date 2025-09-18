using Microsoft.AspNetCore.Http;

namespace BusinessObjects.Dtos.Reward;

public class RewardCreateDto
{
    public int Conditions { get; set; }
    public IFormFile? Url_image { get; set; }
    public int Quantity_box { get; set; }
}