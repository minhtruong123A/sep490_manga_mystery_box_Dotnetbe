using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
using MongoDB.Bson;
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
        if (dto.ImageUrl != null)
        {
            var urlI = await imageService.UploadModeratorProductOrMysteryBoxImageAsync(dto.ImageUrl);
            var mys1 = new MysteryBox();
            mys1.Id = ObjectId.GenerateNewId().ToString();
            mys1.Name = dto.Name;
            mys1.Description = dto.Description;
            mys1.TotalProduct = dto.TotalProduct;
            mys1.Price = dto.Price;
            mys1.UrlImage = urlI;
            mys1.Title = dto.Title;
            await unitOfWork.MysteryBoxRepository.AddAsync(mys1);

            var box1 = new MangaBox();
            box1.Status = 0;
            box1.CreatedAt = DateTime.Now;
            box1.UpdatedAt = DateTime.Now;
            box1.MysteryBoxId = mys1.Id;
            box1.CollectionTopicId = dto.CollectionTopicId;
            box1.Title = dto.Title;
            await unitOfWork.MangaBoxRepository.AddAsync(box1);
            await unitOfWork.SaveChangesAsync();

            return true;
        }

        var url = await mysteryBoxService.GetImageUrlsByCollectionIdAsync(dto.CollectionTopicId);

        var mys = new MysteryBox();
        mys.Id = ObjectId.GenerateNewId().ToString();
        mys.Name = dto.Name;
        mys.Description = dto.Description;
        mys.TotalProduct = dto.TotalProduct;
        mys.Price = dto.Price;
        mys.UrlImage = url;
        mys.Title = dto.Title;
        await unitOfWork.MysteryBoxRepository.AddAsync(mys);

        var box = new MangaBox();
        box.Status = 0;
        box.CreatedAt = DateTime.Now;
        box.UpdatedAt = DateTime.Now;
        box.MysteryBoxId = mys.Id;
        box.CollectionTopicId = dto.CollectionTopicId;
        box.Title = dto.Title;
        await unitOfWork.MangaBoxRepository.AddAsync(box);
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
        if (quantity <= 0) throw new Exception("Quantity must be greater than zero");

        var mangaBox = await unitOfWork.MangaBoxRepository.FindOneAsync(x => x.Id == boxId) ??
                       throw new Exception("Box not found");
        var mysteryBox = await unitOfWork.MysteryBoxRepository.FindOneAsync(x => x.Id == mangaBox.MysteryBoxId) ??
                         throw new Exception("MysteryBox not found");
        var totalPrice = mysteryBox.Price * quantity;
        var user = await unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ??
                   throw new Exception("User not found");
        var walletId = user.WalletId;
        if (string.IsNullOrEmpty(walletId))
        {
            var newWallet = new UseDigitalWallet
            {
                Ammount = 0,
                IsActive = true
            };
            await unitOfWork.UseDigitalWalletRepository.AddAsync(newWallet);
            walletId = newWallet.Id;
            user.WalletId = walletId;
            await unitOfWork.UserRepository.UpdateAsync(user.Id, user);
        }

        var wallet = await unitOfWork.UseDigitalWalletRepository.FindOneAsync(w => w.Id == walletId);
        if (wallet == null || wallet.Ammount < totalPrice) throw new Exception("Insufficient balance");

        wallet.Ammount -= totalPrice;
        await unitOfWork.UseDigitalWalletRepository.UpdateAsync(wallet.Id, wallet);

        var boxOrder = new BoxOrder
        {
            UserId = userId,
            BoxId = boxId,
            Quantity = quantity,
            Amount = totalPrice
        };
        await unitOfWork.BoxOrderRepository.AddAsync(boxOrder);

        var orderHistory = new OrderHistory
        {
            BoxOrderId = boxOrder.Id,
            ProductOrderId = null,
            Datetime = DateTime.UtcNow,
            Status = (int)TransactionStatus.Success
        };
        await unitOfWork.OrderHistoryRepository.AddAsync(orderHistory);

        var paymentSession = new DigitalPaymentSession
        {
            WalletId = walletId,
            OrderId = orderHistory.Id,
            Type = DigitalPaymentSessionType.MysteryBox.ToString(),
            Amount = totalPrice,
            IsWithdraw = false
        };
        await unitOfWork.DigitalPaymentSessionRepository.AddAsync(paymentSession);

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

        await unitOfWork.SaveChangesAsync();

        return orderHistory.Id;
    }
}