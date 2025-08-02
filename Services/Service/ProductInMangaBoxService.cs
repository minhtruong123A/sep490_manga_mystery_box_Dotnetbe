using BusinessObjects;
using BusinessObjects.Dtos.ProductInMangaBox;
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
        public async Task<bool> CreateAsync(string boxId,List<ProductInMangaBoxCreateDto> dtos)
         {
            var box = await _uniUnitOfWork.MangaBoxRepository.GetByIdAsync(boxId);
            foreach(var dto in dtos)
            {
                
                var product = await _uniUnitOfWork.ProductRepository.GetByIdAsync(dto.ProductId.ToString());
                if (product == null) throw new Exception("Product not exist");
                if (box.CollectionTopicId.Equals(product.CollectionId)) 
                {
                    var productInMangaBox = new ProductInMangaBox();
                    productInMangaBox.ProductId = dto.ProductId;
                    productInMangaBox.Chance = dto.Chance;
                    productInMangaBox.MangaBoxId = boxId;
                    productInMangaBox.Name = product.Name;
                    productInMangaBox.Description = product.Description;
                    await _uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
                    await _uniUnitOfWork.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Products not included in this box");
                }
                    
            }
            return true;
        }
    }
}
