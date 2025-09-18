namespace Services.Interface;

public interface ISignedUrlService
{
    Task<string?> GetSignedUrlIfAvailableAsync(string? path);
}