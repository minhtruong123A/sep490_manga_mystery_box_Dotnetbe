using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMangaBoxService
    {
        Task<MangaBox> AddAsync(MangaBox mangaBox);
        Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync();
        Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id);
    }
}
