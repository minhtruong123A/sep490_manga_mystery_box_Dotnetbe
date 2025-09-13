using BusinessObjects;

namespace DataAccessLayers.Interface;

public interface IEmailVerificationRepository : IGenericRepository<EmailVerification>
{
    Task DeleteByEmailAsync(string email);
}