using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace BusinessObjects.Dtos.User
{

    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurentPassword { get; set; }

        [Required(ErrorMessage = "New password is required")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[!@#$%^&*(),.?\\:{}|<>]).{8,15}$",
            ErrorMessage = "New password must be 8-15 characters, include uppercase, lowercase, number and special character")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        public string ConfirmPassword { get; set; }
    }

}
