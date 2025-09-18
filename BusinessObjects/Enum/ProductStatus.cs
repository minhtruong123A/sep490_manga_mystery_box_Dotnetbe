using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Enum
{
    public enum ProductStatus
    {
        Normal = 0,
        Limit = 1,
        Using = 2,
        Using_Limit = 3,
        Reuse = 4,
        Block_Limit = -1
    }
}
