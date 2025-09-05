using Microsoft.AspNetCore.Http;

namespace BusinessObjects.Dtos.User;

public class UserUpdateDto
{
    public IFormFile? UrlImage { get; set; }

/*        public string? Username { get; set; }*/

    public string? PhoneNumber { get; set; }

    public string? AccountBankName { get; set; }

    public string? BankNumber { get; set; }

    public string? BankId { get; set; }
}