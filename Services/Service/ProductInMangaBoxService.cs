using BusinessObjects;
using BusinessObjects.Dtos.ProductInMangaBox;
using DataAccessLayers.Interface;
using Services.Interface;

namespace Services.Service;

public class ProductInMangaBoxService(IUnitOfWork uniUnitOfWork) : IProductInMangaBoxService
{
    public async Task CreateProductInMangaBoxAsync(ProductInMangaBox productInMangaBox)
    {
        await uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
    }

    public async Task<bool> CreateAsync(string boxId, List<ProductInMangaBoxCreateDto> dtos)
    {
        var boxDetail = await uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(boxId);
        var box = await uniUnitOfWork.MangaBoxRepository.GetByIdAsync(boxId);
        if (boxId.Equals("6899caa6250b50c9837a4aec"))
        {
            if (boxDetail.Products.Count() == boxDetail.TotalProduct)
            {
                var mysteryBox = await uniUnitOfWork.MysteryBoxRepository.GetByIdAsync(box.MysteryBoxId);
                mysteryBox.TotalProduct += dtos.Count();
                await uniUnitOfWork.MysteryBoxRepository.UpdateAsync(mysteryBox.Id, mysteryBox);
                await uniUnitOfWork.SaveChangesAsync();
            }
            else if (boxDetail.Products.Count() + dtos.Count() > boxDetail.TotalProduct)
            {
                var total = boxDetail.Products.Count() + dtos.Count();
                var inc = total - boxDetail.TotalProduct;
                var mysteryBox = await uniUnitOfWork.MysteryBoxRepository.GetByIdAsync(box.MysteryBoxId);
                mysteryBox.TotalProduct += inc;
                await uniUnitOfWork.MysteryBoxRepository.UpdateAsync(mysteryBox.Id, mysteryBox);
                await uniUnitOfWork.SaveChangesAsync();
            }

            boxDetail = await uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(boxId);
            box = await uniUnitOfWork.MangaBoxRepository.GetByIdAsync(boxId);
            foreach (var dto in from dto in dtos let exist = boxDetail.Products.Where(x => x.ProductId.Equals(dto.ProductId)).Any() where !exist select dto)
            {
                var productWithRarity =
                    await uniUnitOfWork.ProductRepository.GetProductWithRarityByIdAsync(dto.ProductId);
                var product = await uniUnitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
                if (!productWithRarity.RarityName.Equals("epic") &&
                    !productWithRarity.RarityName.Equals("legendary")) continue;
                var productInMangaBoxs = await uniUnitOfWork.ProductInMangaBoxRepository.GetAllAsync();
                var productInMangaBoxExist = productInMangaBoxs.Any(x => x.ProductId.Equals(productWithRarity.ProductId));
                if (productInMangaBoxExist) continue;
                if (product == null) throw new Exception("Product not exist");
                if (box.CollectionTopicId.Equals(product.CollectionId))
                {
                    var productInMangaBox = new ProductInMangaBox();
                    productInMangaBox.ProductId = dto.ProductId;
                    productInMangaBox.Chance = dto.Chance;
                    productInMangaBox.MangaBoxId = boxId;
                    productInMangaBox.Name = product.Name;
                    productInMangaBox.Description = product.Description;
                    await uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
                }
                else
                {
                    throw new Exception("Products not included in this box");
                }
            }

            return true;
        }

        if (boxDetail.TotalProduct - boxDetail.Products.Count() != dtos.Count) return false;
        {
            foreach (var dto in from dto in dtos let exist = boxDetail.Products.Any(x => x.ProductId.Equals(dto.ProductId)) where !exist select dto)
            {
                var productWithRarity =
                    await uniUnitOfWork.ProductRepository.GetProductWithRarityByIdAsync(dto.ProductId);
                var product = await uniUnitOfWork.ProductRepository.GetByIdAsync(dto.ProductId);
                if (productWithRarity.RarityName.Equals("epic") || productWithRarity.RarityName.Equals("legendary"))
                {
                    var productInMangaBoxs = await uniUnitOfWork.ProductInMangaBoxRepository.GetAllAsync();
                    var productInMangaBoxExist = productInMangaBoxs.Any(x => x.ProductId.Equals(productWithRarity.ProductId));
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
                            await uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
                            boxDetail = await uniUnitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(boxId);
                            if (boxDetail.TotalProduct == boxDetail.Products.Count())
                            {
                                box.Status = 1;
                                await uniUnitOfWork.MangaBoxRepository.UpdateAsync(box.Id, box);
                                await uniUnitOfWork.SaveChangesAsync();
                            }
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
                    await uniUnitOfWork.ProductInMangaBoxRepository.AddAsync(productInMangaBox);
                    box.Status = 1;
                    await uniUnitOfWork.MangaBoxRepository.UpdateAsync(box.Id, box);
                    await uniUnitOfWork.SaveChangesAsync();
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