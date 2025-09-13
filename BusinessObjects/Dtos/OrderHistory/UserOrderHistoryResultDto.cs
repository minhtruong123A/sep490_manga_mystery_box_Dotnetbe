namespace BusinessObjects.Dtos.OrderHistory;

public class UserOrderHistoryResultDto
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public List<OrderHistoryDto> OrderHistories { get; set; }
}