using BusinessObjects.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IUserProductService
    {
        Task<List<CollectionProductsDto>> GetAllWithDetailsAsync(string id, string collectionId);
    }
}
