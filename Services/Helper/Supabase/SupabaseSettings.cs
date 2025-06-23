using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Helper.Supabase
{
    public class SupabaseSettings
    {
        public string Url { get; set; }
        public string ServiceRoleKey { get; set; }
        public string Bucket { get; set; }
        public int SignedUrlExpireSeconds { get; set; }
    }
}
