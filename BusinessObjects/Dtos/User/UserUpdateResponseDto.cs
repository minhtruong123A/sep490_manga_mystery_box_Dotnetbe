using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Dtos.User
{
    public class UserUpdateResponseDto
    {
        public string Username { get; set; }
        public string ProfileImage { get; set; }
        public string PhoneNumber { get; set; }
        public string AccountBankName { get; set; }
        public string BankNumber { get; set; }
        public string Bankid { get; set; }
    }
}
