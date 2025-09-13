using System.Text.Json.Serialization;

namespace Services.Helper.Supabase;

public class SupabaseFileObject
{
    [JsonPropertyName("name")] public string Name { get; set; }
}