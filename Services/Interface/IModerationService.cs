namespace Services.Interface;

public interface IModerationService
{
    //Task<bool> IsContentSafeOpenAIAsync(string content);
    Task<bool> IsContentSafeGeminiAIAsync(string content);
}