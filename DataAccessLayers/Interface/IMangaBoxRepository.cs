using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Interface
{
    public interface IMangaBoxRepository : IGenericRepository<MangaBox>
    {
        //Task<List<MangaBoxDetailDto>> GetAllWithDetailsAsync();
        Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync();
        Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id);
    }
}
