using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enum
{
    public enum ChangePasswordResult
    {
        Success = 1,
        PasswordMismatch = 2,
        InvalidCurrentPassword = 3
    }
}
