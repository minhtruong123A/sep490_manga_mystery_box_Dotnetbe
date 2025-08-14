using BusinessObjects;
using BusinessObjects.Dtos.Product;
using BusinessObjects.Enum;
using BusinessObjects.Mongodb;
using Microsoft.Extensions.Options;
using BusinessObjects.Options;
using DataAccessLayers.Interface;
using DataAccessLayers.Pipelines;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataAccessLayers.Repository
{
    public class SellProductRepository : GenericRepository<SellProduct>, ISellProductRepository
    {
        private readonly IMongoCollection<SellProduct> _sellProductCollection;
        private readonly IMongoCollection<UserProduct> _userProductCollection;
        private readonly IMongoCollection<Product> _productCollection;
        private readonly IMongoCollection<User> _userCollection;
        private readonly IMongoCollection<Collection> _collections;
        private readonly IMongoCollection<Rarity> _rarityCollection;
        private readonly IMongoCollection<ProductOrder> _productOrderCollection;
        private readonly IMongoCollection<UseDigitalWallet> _walletCollection;
        private readonly IMongoCollection<DigitalPaymentSession> _paymentSessionCollection;
        private readonly IMongoCollection<UserCollection> _userCollectionCollection;
        private readonly IMongoCollection<OrderHistory> _orderHistoryCollection;
        private readonly IMongoCollection<ProductInMangaBox> _productInMangaBoxCollection;
        private readonly IMongoCollection<MangaBox> _mangaBoxCollection;
        private readonly IMongoCollection<TransactionFee> _transactionFeeCollection;
        private readonly IUserAchievementRepository _userAchievementRepository;
        private readonly FeeSettings _feeSettings;
        private readonly ProductPriceSettings _priceSettings;

        public SellProductRepository(MongoDbContext context, IOptions<FeeSettings> feeOptions, IOptions<ProductPriceSettings> priceSettings, IUserAchievementRepository userAchievementRepository) : base(context.GetCollection<SellProduct>("SellProduct"))
        {
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _userProductCollection = context.GetCollection<UserProduct>("User_Product");
            _productCollection = context.GetCollection<Product>("Product");
            _userCollection = context.GetCollection<User>("User");
            _collections = context.GetCollection<Collection>("Collection");
            _rarityCollection = context.GetCollection<Rarity>("Rarity");
            _productOrderCollection = context.GetCollection<ProductOrder>("ProductOrder");
            _walletCollection = context.GetCollection<UseDigitalWallet>("UseDigitalWallet");
            _paymentSessionCollection = context.GetCollection<DigitalPaymentSession>("DigitalPaymentSession");
            _userCollectionCollection = context.GetCollection<UserCollection>("UserCollection");
            _orderHistoryCollection = context.GetCollection<OrderHistory>("OrderHistory");
            _productInMangaBoxCollection = context.GetCollection<ProductInMangaBox>("ProductInMangaBox");
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
            _transactionFeeCollection = context.GetCollection<TransactionFee>("TransactionFee");
            _feeSettings = feeOptions.Value;
            _priceSettings = priceSettings.Value;
            _userAchievementRepository = userAchievementRepository;
        }

        public async Task<int> CreateSellProductAsync(SellProductCreateDto dto, string userId)
        {
            if (dto.Quantity <= 0) throw new Exception("Quantity must be greater than 0.");

            if (dto.Price < _priceSettings.MinPrice || dto.Price > _priceSettings.MaxPrice) throw new Exception($"Price must be between {_priceSettings.MinPrice:N0} VND and {_priceSettings.MaxPrice:N0} VND.");

            var filter = Builders<UserProduct>.Filter.And(
                Builders<UserProduct>.Filter.Eq(x => x.Id, dto.UserProductId),
                Builders<UserProduct>.Filter.Eq(x => x.CollectorId, userId),
                Builders<UserProduct>.Filter.Gte(x => x.Quantity, dto.Quantity)
            );
            var update = Builders<UserProduct>.Update.Inc(x => x.Quantity, -dto.Quantity);
            var result = await _userProductCollection.UpdateOneAsync(filter, update);
            if (result.ModifiedCount == 0) throw new Exception("Not enough quantity in inventory or product not found.");

            var updatedUserProduct = await _userProductCollection.Find(x => x.Id == dto.UserProductId).FirstOrDefaultAsync();
            if (updatedUserProduct == null) throw new Exception("Unexpected error: UserProduct not found after update.");

            var ExchangeCode = await GenerateUniqueExchangeCodeAsync();
            var newSellProduct = new SellProduct
            {
                ProductId = updatedUserProduct.ProductId,
                SellerId = userId,
                Quantity = dto.Quantity,
                Description = dto.Description,
                Price = dto.Price,
                ExchangeCode = ExchangeCode,
                IsSell = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _sellProductCollection.InsertOneAsync(newSellProduct);

            return ExchangeCode;
        }


        public async Task<bool> UpdateSellProductAsync(UpdateSellProductDto dto)
        {
            var sellProduct = await _sellProductCollection.Find(x => x.Id.Equals(dto.Id)).FirstOrDefaultAsync();
            if (sellProduct.IsSell == true)
            {
                return false;
            }
            var filter = Builders<SellProduct>.Filter.Eq(x => x.Id, dto.Id);
            var update = Builders<SellProduct>.Update
                .Set(x => x.Description, dto.Description)
                .Set(x => x.Price, dto.Price)
                .Set(x => x.UpdatedAt, DateTime.Now);

            var result = await _sellProductCollection.UpdateOneAsync(filter, update);

            return true;
        }

        public async Task<bool> ChangestatusSellProductAsync(string id)
        {
            var filter = Builders<SellProduct>.Filter.Eq(x => x.Id, id);
            bool status;
            var sellProduct = await _sellProductCollection.Find(x=>x.Id.Equals(id)).FirstOrDefaultAsync();
            if(sellProduct.Quantity <= 0)
            {
                return false;
            }
            if(sellProduct.IsSell == true)
            {
                status = false;
            }
            else
            {
                status = true;
            }

            var update = Builders<SellProduct>.Update
                .Set(x => x.IsSell, status);
                

            var result = await _sellProductCollection.UpdateOneAsync(filter, update);

            return true;
        }

        private async Task<int> GenerateUniqueExchangeCodeAsync()
        {
            var rng = new Random();
            int code;
            bool exists;

            do
            {
                code = rng.Next(100000, 1000000);
                exists = await _sellProductCollection.Find(sp => sp.ExchangeCode == code).AnyAsync();
            } while (exists);

            return code;
        }

        //getallproductonsale
        //public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        //{
        //    return await _sellProductCollection
        //        .WithBson()
        //        .RunAggregateWithLookups(
        //            buildPipeline: SellProductPipelineBuilder.BuildProductOnSalePipeline,
        //            selector: x => new SellProductGetAllDto
        //            {
        //                Id = x.GetValue("Id", "").AsString,
        //                Name = x.GetValue("Name", "").AsString,
        //                Price = x.GetValue("Price", 0).ToInt32(),
        //                Username = x.GetValue("Username", "").AsString,
        //                Topic = x.TryGetString("Topic") ?? "Unknown",
        //                UrlImage = x.TryGetString("UrlImage")
        //            });
        //}
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleAsync()
        {
            var sellProductList = await _sellProductCollection.AsQueryable().Where(c => c.IsSell).ToListAsync();
            var productIds = sellProductList.Select(c => c.ProductId).ToHashSet();
            var sellerIds = sellProductList.Select(c => c.SellerId).ToHashSet();
            var productTask = _productCollection.AsQueryable().Where(c => productIds.Contains(c.Id.ToString())).ToListAsync();
            var userTask = _userCollection.AsQueryable().Where(c => sellerIds.Contains(c.Id.ToString())).ToListAsync();
            var userProductTask = _userProductCollection.AsQueryable().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var userCollectionTask = _userCollectionCollection.AsQueryable().Where(uc => sellerIds.Contains(uc.UserId)).ToListAsync();
            await Task.WhenAll(productTask, userTask, userProductTask, userCollectionTask);

            var productList = productTask.Result;
            var userList = userTask.Result;
            var userProductList = userProductTask.Result;
            var userCollectionList = userCollectionTask.Result;

            var collectionIds = userCollectionList.Select(up => up.CollectionId).ToHashSet();
            var rarityIds = productList.Select(up => up.RarityId).ToHashSet();
            var collectionsTask = _collections.AsQueryable().Where(c => collectionIds.Contains(c.Id.ToString())).ToListAsync();
            var raritiesTask = _rarityCollection.AsQueryable().Where(r => rarityIds.Contains(r.Id.ToString())).ToListAsync();
            await Task.WhenAll(collectionsTask, raritiesTask);

            var collections = collectionsTask.Result;
            var rarities = raritiesTask.Result;

            return sellProductList.Select(sellProduct =>
            {
                var product = productList.FirstOrDefault(c => c.Id.ToString() == sellProduct.ProductId);
                var user = userList.FirstOrDefault(c => c.Id.ToString() == sellProduct.SellerId);
                var userProduct = userProductList.FirstOrDefault(c => c.ProductId == sellProduct.ProductId);
                var userCollection = userCollectionList.FirstOrDefault(uc => uc.UserId == sellProduct.SellerId);
                var collection = userCollection != null
                       ? collections.FirstOrDefault(c => c.Id.ToString() == userCollection.CollectionId)
                       : null;
                var rarity = product != null
                       ? rarities.FirstOrDefault(r => r.Id.ToString() == product.RarityId)
                       : null;

                return new SellProductGetAllDto
                {
                    Id = sellProduct.Id.ToString(),
                    Name = product?.Name ?? "Unknown",
                    Price = sellProduct?.Price ?? null,
                    UserId = user?.Id ?? "Unknown",
                    Username = user?.Username ?? "Unknown",
                    Topic = collection?.Topic ?? "Unknown",
                    Quantity = sellProduct?.Quantity ?? 0,
                    UrlImage = product?.UrlImage ?? "Unknown",
                    RarityName = rarity?.Name ?? "Unknown",
                    CreatedAt = sellProduct?.CreatedAt ?? null,
                    IsSell = sellProduct?.IsSell ?? null,
                };
            }).ToList();
        }
        public async Task<List<SellProductGetAllDto>> GetAllProductOnSaleOfUserIdAsync(string id)
        {  //c.IsSell && 
            var sellProductList = await _sellProductCollection.AsQueryable().Where(c => c.SellerId.Equals(id) && c.Quantity > 0).ToListAsync();
            var productIds = sellProductList.Select(c => c.ProductId).ToHashSet();
            var sellerIds = sellProductList.Select(c => c.SellerId).ToHashSet();
            var productTask = _productCollection.AsQueryable().Where(c => productIds.Contains(c.Id.ToString())).ToListAsync();
            var userTask = _userCollection.AsQueryable().Where(c => sellerIds.Contains(c.Id.ToString())).ToListAsync();
            var userProductTask = _userProductCollection.AsQueryable().Where(c => productIds.Contains(c.ProductId)).ToListAsync();
            var userCollectionTask = _userCollectionCollection.AsQueryable().Where(uc => sellerIds.Contains(uc.UserId)).ToListAsync();
            await Task.WhenAll(productTask, userTask, userProductTask, userCollectionTask);

            var productList = productTask.Result;
            var userList = userTask.Result;
            var userProductList = userProductTask.Result;
            var userCollectionList = userCollectionTask.Result;

            var collectionIds = userCollectionList.Select(uc => uc.CollectionId).ToHashSet();
            var collections = await _collections.AsQueryable().Where(c => collectionIds.Contains(c.Id.ToString())).ToListAsync();
            var rarityIds = productList.Select(up => up.RarityId).ToHashSet();
            var rarities = await _rarityCollection.AsQueryable().Where(r => rarityIds.Contains(r.Id.ToString())).ToListAsync();

            return sellProductList.Select(sellProduct =>
            {
                var product = productList.FirstOrDefault(c => c.Id.ToString() == sellProduct.ProductId);
                var user = userList.FirstOrDefault(c => c.Id.ToString() == sellProduct.SellerId);
                var userProduct = userProductList.FirstOrDefault(c => c.ProductId == sellProduct.ProductId);
                var userCollection = userCollectionList.FirstOrDefault(uc => uc.UserId == sellProduct.SellerId);
                var collection = userCollection != null
                                ? collections.FirstOrDefault(c => c.Id.ToString() == userCollection.CollectionId)
                                : null; 
                var rarity = product != null
                                ? rarities.FirstOrDefault(r => r.Id.ToString() == product.RarityId)
                                : null;

                return new SellProductGetAllDto
                {
                    Id = sellProduct.Id.ToString(),
                    Name = product?.Name ?? "Unknown",
                    Price = sellProduct.Price,
                    UserId = user?.Id ?? "Unknown",
                    Username = user?.Username ?? "Unknown",
                    Topic = collection?.Topic ?? "Unknown",
                    RarityName = rarity?.Name ?? "Unknown",
                    Quantity = sellProduct?.Quantity ?? 0,
                    UrlImage = product?.UrlImage ?? "Unknown",
                    CreatedAt = sellProduct?.CreatedAt ?? null,
                    IsSell = sellProduct?.IsSell ?? null,
                };
            }).ToList();
        }
        //getproductonsalebyid
        //public async Task<SellProductDetailDto> GetProductDetailByIdAsync(string id)
        //{
        //    var objectId = ObjectId.Parse(id);

        //    var result = await _sellProductCollection
        //        .WithBson()
        //        .RunAggregateWithLookups(
        //            buildPipeline: p => SellProductPipelineBuilder.BuildProductDetailPipeline(p, objectId),
        //            selector: x => new SellProductDetailDto
        //            {
        //                Id = x.GetValue("Id", "").AsString,
        //                Name = x.GetValue("Name", "").AsString,
        //                Price = x.GetValue("Price", 0).ToInt32(),
        //                Username = x.GetValue("Username", "").AsString,
        //                Topic = x.TryGetString("Topic") ?? "Unknown",
        //                UrlImage = x.TryGetString("UrlImage"),
        //                RateName = x.TryGetString("RateName") ?? "Unknown",
        //                Description = x.TryGetString("Description") ?? ""
        //            });

        //    return result.FirstOrDefault();
        public async Task<SellProductDetailDto?> GetProductDetailByIdAsync(string id)
        {
            //&& c.IsSell
            var sellProduct = await _sellProductCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == id);
            if (sellProduct is null) return null;
            var productTask = _productCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.ProductId);
            var userTask = _userCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == sellProduct.SellerId);
            var userProductTask = _userProductCollection.AsQueryable().FirstOrDefaultAsync(c => c.ProductId == sellProduct.ProductId);
            var userCollectionTask = _userCollectionCollection.AsQueryable().Where(uc => uc.UserId.Contains(uc.UserId)).ToListAsync();
            await Task.WhenAll(productTask, userTask, userProductTask, userCollectionTask);

            var productResult = productTask.Result;
            var userResult = userTask.Result;
            var userProductResult = userProductTask.Result;
            var userCollectionList = userCollectionTask.Result;
            var firstUserCollection = userCollectionList.FirstOrDefault();

            var collectionTask = firstUserCollection?.CollectionId is string colId
                ? _collections.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == colId) :  Task.FromResult<Collection?>(null);
            var rarityTask = productResult?.RarityId is string rId
                ? _rarityCollection.AsQueryable().FirstOrDefaultAsync(c => c.Id.ToString() == rId) : Task.FromResult<Rarity?>(null);
            await Task.WhenAll(collectionTask, rarityTask);

            var collectionResult = collectionTask.Result;
            var rarityResult = rarityTask.Result;

            return new SellProductDetailDto
            {
                Id = sellProduct.Id.ToString(),
                Name = productResult?.Name ?? "Unknown",
                Price = sellProduct.Price,
                UrlImage = productResult?.UrlImage ?? "Unknown",
                Description = sellProduct.Description ?? "",
                UserId = userResult?.Id ?? "Unknown Id",
                Username = userResult?.Username ?? "Unknown",
                UserProfileImage = userResult?.ProfileImage ?? "",
                Topic = collectionResult?.Topic ?? "Unknown",
                RateName = rarityResult?.Name ?? "Unknown",
                Quantity = sellProduct.Quantity,
                IsSell = sellProduct?.IsSell ?? null,
            };
        }

        public async Task<string> BuySellProductAsync(string buyerId, string sellProductId, int quantity)
        {
            ValidateQuantity(quantity);

            var sellProduct = await GetSellProductAsync(sellProductId);
            ValidateBuyer(sellProduct, buyerId);
            ValidateStock(sellProduct, quantity);

            var buyer = await GetUserAsync(buyerId);
            var buyerWallet = await GetWalletAsync(buyer.WalletId);
            int totalPrice = sellProduct.Price * quantity;
            EnsureSufficientFunds(buyerWallet, totalPrice);

            var seller = await GetUserAsync(sellProduct.SellerId);
            var sellerWallet = await GetWalletAsync(seller.WalletId);

            var feeInfo = CalculateFee(totalPrice);
            await UpdateSellProductStockAsync(sellProduct, quantity);
            await ProcessWalletsAsync(buyerWallet, sellerWallet, feeInfo);

            var (buyerOrder, buyerHistory, buyerPayment) = await CreateBuyerRecordsAsync(buyerId, sellProduct, totalPrice, buyerWallet.Id);
            var (sellerOrder, sellerHistory, sellerPayment) = await CreateSellerRecordsAsync(seller.Id, buyerId, sellProduct, feeInfo, sellerWallet.Id);
            await LogTransactionFeeAsync(seller.Id, sellProduct.ProductId, sellerHistory.Id, feeInfo);

            await UpdateUserCollectionAndProductAsync(buyerId, sellProduct.ProductId, quantity);

            await _userAchievementRepository.CheckAchievement(buyerId);
            return buyerHistory.Id;
        }

        //Helper Methods Of Buy Product DO NOT FIX IT
        private void ValidateQuantity(int quantity)
        {
            if (quantity <= 0) throw new Exception("Quantity must be greater than 0");
        }

        private void ValidateBuyer(SellProduct product, string buyerId)
        {
            if (product.SellerId == buyerId) throw new Exception("You cannot buy your own product");
        }

        private void ValidateStock(SellProduct product, int quantity)
        {
            if (!product.IsSell || product.Quantity < quantity) throw new Exception("Not enough product quantity available for sale");
        }

        private void EnsureSufficientFunds(UseDigitalWallet wallet, int totalPrice)
        {
            if (wallet.Ammount < totalPrice) throw new Exception("Insufficient balance");
        }

        private async Task<SellProduct> GetSellProductAsync(string id) => await _sellProductCollection.Find(x => x.Id == id).FirstOrDefaultAsync() ?? throw new Exception("SellProduct not found");

        private async Task<User> GetUserAsync(string userId) => await _userCollection.Find(x => x.Id == userId).FirstOrDefaultAsync() ?? throw new Exception("User not found");

        private async Task<UseDigitalWallet> GetWalletAsync(string walletId) => await _walletCollection.Find(x => x.Id == walletId).FirstOrDefaultAsync() ?? throw new Exception("Wallet not found");

        private (double rate, int fee, int net) CalculateFee(int total)
        {
            double rate = _feeSettings.BuyFeeRate;
            int fee = (int)(total * rate);
            return (rate, fee, total - fee);
        }

        private async Task ProcessWalletsAsync(UseDigitalWallet buyerWallet, UseDigitalWallet sellerWallet, (double rate, int fee, int net) feeInfo)
        {
            buyerWallet.Ammount -= (feeInfo.fee + feeInfo.net);
            sellerWallet.Ammount += feeInfo.net;
            await _walletCollection.ReplaceOneAsync(x => x.Id == buyerWallet.Id, buyerWallet);
            await _walletCollection.ReplaceOneAsync(x => x.Id == sellerWallet.Id, sellerWallet);
        }

        private async Task UpdateSellProductStockAsync(SellProduct product, int quantity)
        {
            //product.Quantity -= quantity;
            //if (product.Quantity == 0)
            //{
            //    product.IsSell = false;
            //    product.UpdatedAt = DateTime.UtcNow;
            //}
            //await _sellProductCollection.ReplaceOneAsync(x => x.Id == product.Id, product);
            var filter = Builders<SellProduct>.Filter.And(
                Builders<SellProduct>.Filter.Eq(p => p.Id, product.Id),
                Builders<SellProduct>.Filter.Gte(p => p.Quantity, quantity)
            );
            var update = Builders<SellProduct>.Update.Inc(p => p.Quantity, -quantity);
            var result = await _sellProductCollection.UpdateOneAsync(filter, update);
            if (result.ModifiedCount == 0) throw new Exception("The product is either out of stock or no longer available for purchase.");

            var finalFilter = Builders<SellProduct>.Filter.And(
                Builders<SellProduct>.Filter.Eq(p => p.Id, product.Id),
                Builders<SellProduct>.Filter.Eq(p => p.Quantity, 0)
            );
            var finalUpdate = Builders<SellProduct>.Update.Set(p => p.IsSell, false);
            await _sellProductCollection.UpdateOneAsync(finalFilter, finalUpdate);
        }

        private async Task<(ProductOrder, OrderHistory, DigitalPaymentSession)> CreateBuyerRecordsAsync(string buyerId, SellProduct product, int total, string buyerWalletId)
        {
            var order = new ProductOrder
            {
                SellerId = product.SellerId,
                BuyerId = buyerId,
                SellProductId = product.Id,
                Amount = total,
            };
            await _productOrderCollection.InsertOneAsync(order);

            var history = new OrderHistory
            {
                ProductOrderId = order.Id,
                Datetime = DateTime.UtcNow,
                Status = (int)TransactionStatus.Success
            };
            await _orderHistoryCollection.InsertOneAsync(history);

            var payment = new DigitalPaymentSession
            {
                WalletId = buyerWalletId,
                OrderId = history.Id,
                Type = DigitalPaymentSessionType.SellProduct.ToString(),
                Amount = total,
                IsWithdraw = false
            };
            await _paymentSessionCollection.InsertOneAsync(payment);

            return (order, history, payment);
        }

        private async Task<(ProductOrder, OrderHistory, DigitalPaymentSession)> CreateSellerRecordsAsync(string sellerId, string buyerId, SellProduct product, (double rate, int fee, int net) feeInfo, string sellerWalletId)
        {
            var order = new ProductOrder
            {
                SellerId = sellerId,
                BuyerId = buyerId,
                SellProductId = product.Id,
                Amount = feeInfo.net,
            };
            await _productOrderCollection.InsertOneAsync(order);

            var history = new OrderHistory
            {
                ProductOrderId = order.Id,
                Datetime = DateTime.UtcNow,
                Status = (int)TransactionStatus.Success
            };
            await _orderHistoryCollection.InsertOneAsync(history);

            var payment = new DigitalPaymentSession
            {
                WalletId = sellerWalletId,
                OrderId = history.Id,
                Type = DigitalPaymentSessionType.SellProduct.ToString(),
                Amount = feeInfo.net,
                IsWithdraw = true
            };
            await _paymentSessionCollection.InsertOneAsync(payment);

            return (order, history, payment);
        }

        private async Task LogTransactionFeeAsync(string fromUserId, string productId, string referenceId, (double rate, int fee, int net) feeInfo)
        {
            var fee = new TransactionFee
            {
                ReferenceId = referenceId,
                ReferenceType = "Order",
                FromUserId = fromUserId,
                ProductId = productId,
                GrossAmount = feeInfo.net + feeInfo.fee,
                FeeAmount = feeInfo.fee,
                FeeRate = feeInfo.rate,
                Type = "SellProduct",
                CreatedAt = DateTime.UtcNow
            };
            await _transactionFeeCollection.InsertOneAsync(fee);
        }

        private async Task UpdateUserCollectionAndProductAsync(string buyerId, string productId, int quantity)
        {
            var productInBox = await _productInMangaBoxCollection.Find(p => p.ProductId == productId).FirstOrDefaultAsync() ?? throw new Exception("ProductInMangaBox not found");
            var mangaBox = await _mangaBoxCollection.Find(m => m.Id == productInBox.MangaBoxId).FirstOrDefaultAsync() ?? throw new Exception("MangaBox not found");
            var collectionId = mangaBox.CollectionTopicId;
            var userCollection = await _userCollectionCollection.Find(c => c.UserId == buyerId && c.CollectionId == collectionId).FirstOrDefaultAsync();
            if (userCollection == null)
            {
                userCollection = new UserCollection { UserId = buyerId, CollectionId = collectionId };
                await _userCollectionCollection.InsertOneAsync(userCollection);
            }

            var userProduct = await _userProductCollection.Find(up => up.CollectorId == buyerId && up.ProductId == productId).FirstOrDefaultAsync();
            if (userProduct != null)
            {
                var update = Builders<UserProduct>.Update.Inc(x => x.Quantity, quantity);
                await _userProductCollection.UpdateOneAsync(x => x.Id == userProduct.Id, update);
            }
            else
            {
                var newUserProduct = new UserProduct
                {
                    ProductId = productId,
                    CollectorId = buyerId,
                    Quantity = quantity,
                    CollectedAt = DateTime.UtcNow,
                    CollectionId = userCollection.Id
                };
                await _userProductCollection.InsertOneAsync(newUserProduct);
            }
        }
    }
}