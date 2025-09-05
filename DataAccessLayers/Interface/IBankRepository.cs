using BusinessObjects;

namespace DataAccessLayers.Interface;

public interface IBankRepository : IGenericRepository<Bank>
{
    Task<Bank> GetBankByAbbreviation(string abbreviation);
}