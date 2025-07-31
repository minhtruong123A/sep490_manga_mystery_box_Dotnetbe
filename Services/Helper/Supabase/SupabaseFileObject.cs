using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Services.Helper.Supabase
{
    public class SupabaseFileObject
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
