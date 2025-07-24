using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using Microsoft.AspNetCore.Http;
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
        Task<bool> CreateNewMangaBoxAsync(MangaBoxCreateDto dto);
        Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync();
        Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id);
        Task<string> BuyBoxAsync(string userId, string boxId, int quantity);
    }
}
