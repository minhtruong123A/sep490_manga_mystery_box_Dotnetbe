using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface ICollectionService
    {
        Task<List<Collection>> GetAllAsync();
        Task<int> CreateCollectionAsync(string topic);
    }
}
