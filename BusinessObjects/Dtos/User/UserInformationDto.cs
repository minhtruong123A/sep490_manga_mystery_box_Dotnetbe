namespace BusinessObjects.Dtos.User;

public class UserInformationDto
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string ProfileImage { get; set; }
    public string PhoneNumber { get; set; }
    public string WalletId { get; set; }
    public string BankId { get; set; }
    public string AccountBankName { get; set; }
    public string Banknumber { get; set; }
    public DateTime CreateDate { get; set; }
    public bool EmailVerification { get; set; }
}