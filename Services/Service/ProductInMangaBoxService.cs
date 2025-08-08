using BusinessObjects;
using BusinessObjects.Dtos.ProductInMangaBox;
using DataAccessLayers.Interface;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

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
            var boxDetail = await _uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(boxId);
            var box = await _uniUnitOfWork.MangaBoxRepository.GetByIdAsync(boxId);
            
                if ((boxDetail.TotalProduct - boxDetail.Products.Count()) == dtos.Count())
                {
                    foreach (var dto in dtos)
                    {
                        var exist = boxDetail.Products.Where(x=>x.ProductId.Equals(dto.ProductId)).Any();
                        if (!exist)
                        {
                            var productWithRarity = await _uniUnitOfWork.ProductRepository.GetProductWithRarityByIdAsync(dto.ProductId.ToString());
                            var product = await _uniUnitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
                            if(productWithRarity.RarityName.Equals("epic") || productWithRarity.RarityName.Equals("legendary"))
                            {
                            var productInMangaBoxs = await _uniUnitOfWork.ProductInMangaBoxRepository.GetAllAsync();
                            var productInMangaBoxExist = productInMangaBoxs.Where(x => x.ProductId.Equals(productWithRarity.ProductId)).Any();
                                if (!productInMangaBoxExist)
                                {
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
                                        box.Status = 1;
                                        await _uniUnitOfWork.MangaBoxRepository.UpdateAsync(box.Id, box);
                                        await _uniUnitOfWork.SaveChangesAsync();
                                    }
                                    else
                                    {
                                        throw new Exception("Products not included in this box");
                                    }
                                }
                            }

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
                                box.Status = 1;
                                await _uniUnitOfWork.MangaBoxRepository.UpdateAsync(box.Id, box);
                                await _uniUnitOfWork.SaveChangesAsync();
                            }
                            else
                            {
                                throw new Exception("Products not included in this box");
                            }
                        }

                    }
                    return true;
                }
            
             return false;
            }
            
        }
    }

