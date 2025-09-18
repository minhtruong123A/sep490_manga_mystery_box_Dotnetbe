namespace BusinessObjects.Dtos.Feedback;

public class FeedbackCreateDto
{
    public string Exchange_infoId { get; set; }
    public string Content { get; set; }
    public float Rating { get; set; }
}