using BusinessObjects;
using BusinessObjects.Dtos.ProductInMangaBox;

namespace Services.Interface;

public interface IProductInMangaBoxService
{
    Task CreateProductInMangaBoxAsync(ProductInMangaBox productInMangaBox);
    Task<bool> CreateAsync(string boxId, List<ProductInMangaBoxCreateDto> dtos);
}