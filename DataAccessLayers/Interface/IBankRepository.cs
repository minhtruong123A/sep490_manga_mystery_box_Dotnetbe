using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IBankRepository : IGenericRepository<Bank>
    {
        Task<Bank> GetBankByAbbreviation(string abbreviation);
    }
}
