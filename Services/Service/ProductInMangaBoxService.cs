using BusinessObjects;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class ProductInMangaBoxService : IProductInMangaBoxService
    {
        private readonly IUnitOfWork _uniUnitOfWork;

        public ProductInMangaBoxService(IUnitOfWork uniUnitOfWork)
        {
            _uniUnitOfWork = uniUnitOfWork;
        }

        public async Task CreateProductInMangaBoxAsync(ProductInMangaBox productInMangaBox) => await _uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
    }
}
