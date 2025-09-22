using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.Interface;

namespace Services.Service;

public class MangaBoxService(IUnitOfWork unitOfWork, IImageService imageService, IMysteryBoxService mysteryBoxService)
    : IMangaBoxService
{
    public async Task<MangaBox> AddAsync(MangaBox mangaBox)
    {
        return await unitOfWork.MangaBoxRepository.AddAsync(mangaBox);
    }

    public async Task<bool> CreateNewMangaBoxAsync(MangaBoxCreateDto dto)
    {
        var imageUrl = dto.ImageUrl != null
            ? await imageService.UploadModeratorProductOrMysteryBoxImageAsync(dto.ImageUrl)
            : await mysteryBoxService.GetImageUrlsByCollectionIdAsync(dto.CollectionTopicId);
        var mysteryBox = CreateMysteryBox(dto, imageUrl);
        await unitOfWork.MysteryBoxRepository.AddAsync(mysteryBox);
        
        var mangaBox = CreateMangaBox(dto, mysteryBox.Id);
        await unitOfWork.MangaBoxRepository.AddAsync(mangaBox);
        await unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync()
    {
        return await unitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
    }

    public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id)
    {
        return await unitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(id);
    }

    public async Task<string> BuyBoxAsync(string userId, string boxId, int quantity)
    {
        ValidateQuantity(quantity);

        var mangaBox = await GetMangaBoxAsync(boxId);
        var mysteryBox = await GetMysteryBoxAsync(mangaBox.MysteryBoxId);
        var totalPrice = mysteryBox.Price * quantity;
        var user = await GetOrCreateUserWalletAsync(userId);

        await DeductBalanceAsync(user, totalPrice);

        var boxOrder = await CreateBoxOrderAsync(userId, boxId, quantity, totalPrice);
        var orderHistory = await CreateOrderHistoryAsync(boxOrder.Id);

        await CreatePaymentSessionAsync(user.WalletId!, orderHistory.Id, totalPrice);
        await UpdateUserBoxAsync(userId, boxId, quantity);

        await unitOfWork.SaveChangesAsync();

        return orderHistory.Id;
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new Exception("Quantity must be greater than zero");
    }

    private async Task<MangaBox> GetMangaBoxAsync(string boxId)
    {
        return await unitOfWork.MangaBoxRepository.FindOneAsync(x => x.Id == boxId)
               ?? throw new Exception("Box not found");
    }

    private async Task<MysteryBox> GetMysteryBoxAsync(string mysteryBoxId)
    {
        return await unitOfWork.MysteryBoxRepository.FindOneAsync(x => x.Id == mysteryBoxId)
               ?? throw new Exception("MysteryBox not found");
    }

    private async Task<User> GetOrCreateUserWalletAsync(string userId)
    {
        var user = await unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId)
                   ?? throw new Exception("User not found");

        if (!string.IsNullOrEmpty(user.WalletId)) return user;
        var newWallet = new UseDigitalWallet
        {
            Ammount = 0,
            IsActive = true
        };
        await unitOfWork.UseDigitalWalletRepository.AddAsync(newWallet);

        user.WalletId = newWallet.Id;
        await unitOfWork.UserRepository.UpdateAsync(user.Id, user);

        return user;
    }

    private async Task DeductBalanceAsync(User user, decimal totalPrice)
    {
        var wallet = await unitOfWork.UseDigitalWalletRepository.FindOneAsync(w => w.Id == user.WalletId);
        if (wallet == null || wallet.Ammount < totalPrice)
            throw new Exception("Insufficient balance");

        wallet.Ammount -= totalPrice;
        await unitOfWork.UseDigitalWalletRepository.UpdateAsync(wallet.Id, wallet);
    }

    private async Task<BoxOrder> CreateBoxOrderAsync(string userId, string boxId, int quantity, int totalPrice)
    {
        var boxOrder = new BoxOrder
        {
            UserId = userId,
            BoxId = boxId,
            Quantity = quantity,
            Amount = totalPrice
        };

        await unitOfWork.BoxOrderRepository.AddAsync(boxOrder);
        return boxOrder;
    }
    
    private static MysteryBox CreateMysteryBox(MangaBoxCreateDto dto, string imageUrl)
    {
        return new MysteryBox
        {
            Id = ObjectId.GenerateNewId().ToString(),
            Name = dto.Name,
            Description = dto.Description,
            TotalProduct = dto.TotalProduct,
            Price = dto.Price,
            UrlImage = imageUrl,
            Title = dto.Title
        };
    }

    private static MangaBox CreateMangaBox(MangaBoxCreateDto dto, string mysteryBoxId)
    {
        return new MangaBox
        {
            Status = 0,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            MysteryBoxId = mysteryBoxId,
            CollectionTopicId = dto.CollectionTopicId,
            Start_time = dto.Start_time,
            End_time = dto.End_time,
            Quantity = dto.Quantity,
            Title = dto.Title
        };
    }

    private async Task<OrderHistory> CreateOrderHistoryAsync(string boxOrderId)
    {
        var orderHistory = new OrderHistory
        {
            BoxOrderId = boxOrderId,
            ProductOrderId = null,
            Datetime = DateTime.UtcNow,
            Status = (int)TransactionStatus.Success
        };

        await unitOfWork.OrderHistoryRepository.AddAsync(orderHistory);
        return orderHistory;
    }

    private async Task CreatePaymentSessionAsync(string walletId, string orderId, int totalPrice)
    {
        var paymentSession = new DigitalPaymentSession
        {
            WalletId = walletId,
            OrderId = orderId,
            Type = DigitalPaymentSessionType.MysteryBox.ToString(),
            Amount = totalPrice,
            IsWithdraw = false
        };

        await unitOfWork.DigitalPaymentSessionRepository.AddAsync(paymentSession);
    }

    private async Task UpdateUserBoxAsync(string userId, string boxId, int quantity)
    {
        var mangaBox = await unitOfWork.MangaBoxRepository.FindOneAsync(x => x.Id == boxId);
        if (mangaBox == null) throw new Exception("MangaBox not found.");
        if (mangaBox.Status != 1) throw new Exception("This MangaBox is not available for purchase.");
        if (mangaBox.End_time <= DateTime.UtcNow) throw new Exception("This MangaBox has expired.");
        if (mangaBox.Quantity < quantity) throw new Exception("Not enough quantity available.");

        var now = DateTime.UtcNow;
        var filter = Builders<MangaBox>.Filter.And(
            Builders<MangaBox>.Filter.Eq(b => b.Id, boxId),
            Builders<MangaBox>.Filter.Gte(b => b.Quantity, quantity),
            Builders<MangaBox>.Filter.Eq(b => b.Status, 1)
        );
        var update = Builders<MangaBox>.Update
            .Inc(b => b.Quantity, -quantity)
            .Set(b => b.UpdatedAt, now);
        var result = await unitOfWork.MangaBoxRepository.UpdateFieldAsync(filter, update);
        if (result.ModifiedCount == 0) throw new Exception("Failed to update MangaBox: it may have been modified or stock insufficient.");

        var zeroStockFilter = Builders<MangaBox>.Filter.And(
            Builders<MangaBox>.Filter.Eq(b => b.Id, boxId),
            Builders<MangaBox>.Filter.Eq(b => b.Status, 1),
            Builders<MangaBox>.Filter.Lte(b => b.Quantity, 0)
        );
        var zeroStockUpdate = Builders<MangaBox>.Update.Set(b => b.Status, 0);
        await unitOfWork.MangaBoxRepository.UpdateFieldAsync(zeroStockFilter, zeroStockUpdate);

        var userBox = await unitOfWork.UserBoxRepository.FindOneAsync(x => x.UserId == userId && x.BoxId == boxId);
        if (userBox == null)
        {
            var newUserBox = new UserBox
            {
                UserId = userId,
                BoxId = boxId,
                Quantity = quantity,
                UpdatedAt = DateTime.UtcNow
            };
            await unitOfWork.UserBoxRepository.AddAsync(newUserBox);
        }
        else
        {
            userBox.Quantity += quantity;
            userBox.UpdatedAt = DateTime.UtcNow;
            await unitOfWork.UserBoxRepository.UpdateAsync(userBox.Id, userBox);
        }
    }
}