using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;

namespace Services.Interface;

public interface IMangaBoxService
{
    Task<MangaBox> AddAsync(MangaBox mangaBox);
    Task<bool> CreateNewMangaBoxAsync(MangaBoxCreateDto dto);
    Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync();
    Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id);
    Task<string> BuyBoxAsync(string userId, string boxId, int quantity);
    Task<bool> DeleteMangaBoxNotUse(string mangaBoxId);
}