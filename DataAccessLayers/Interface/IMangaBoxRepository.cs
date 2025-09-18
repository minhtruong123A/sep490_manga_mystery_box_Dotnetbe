using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;

namespace DataAccessLayers.Interface;

public interface IMangaBoxRepository : IGenericRepository<MangaBox>
{
    //Task<List<MangaBoxDetailDto>> GetAllWithDetailsAsync();
    Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync();
    Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id);
    Task<MysteryBox?> FindMysteryBoxByUrlImageAsync(string urlImage);
}