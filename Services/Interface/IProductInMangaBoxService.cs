using BusinessObjects;
using BusinessObjects.Dtos.ProductInMangaBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IProductInMangaBoxService
    {
        Task CreateProductInMangaBoxAsync(ProductInMangaBox productInMangaBox);
        Task<bool> CreateAsync(string boxId, List<ProductInMangaBoxCreateDto> dtos);
    }
}
