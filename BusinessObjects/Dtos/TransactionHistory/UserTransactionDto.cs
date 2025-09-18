namespace BusinessObjects.Dtos.TransactionHistory;

public class UserTransactionDto
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string WalletId { get; set; }
    public List<TransactionHistoryDto> Transactions { get; set; }
}