namespace BusinessObjects.Dtos.TransactionFee;

public class TransactionFeeDto
{
    public string Id { get; set; }
    public string ReferenceId { get; set; }
    public string ReferenceType { get; set; }


    public string FromUserId { get; set; }
    public string Username { get; set; }
    public string ProfileImage { get; set; }

    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public string RarityId { get; set; }
    public string UrlImage { get; set; }
    public bool Is_Block { get; set; }

    public string Type { get; set; }
    public int GrossAmount { get; set; }
    public int FeeAmount { get; set; }
    public double FeeRate { get; set; }
    public DateTime CreatedAt { get; set; }
}