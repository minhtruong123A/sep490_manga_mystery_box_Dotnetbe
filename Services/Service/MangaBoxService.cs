using AutoMapper;
using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Enum;
using DataAccessLayers.Interface;
using DataAccessLayers.UnitOfWork;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Services.Helper.Supabase;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Service
{
    public class MangaBoxService : IMangaBoxService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        public MangaBoxService(IUnitOfWork unitOfWork, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
        }

        public async Task<MangaBox> AddAsync(MangaBox mangaBox) => await _unitOfWork.MangaBoxRepository.AddAsync(mangaBox);
        public async Task<bool> CreateNewMangaBoxAsync(MangaBoxCreateDto dto)
        {
            var url = await _imageService.UploadModeratorProductOrMysteryBoxImageAsync(dto.ImageUrl);
            var mys = new MysteryBox();
            mys.Id = ObjectId.GenerateNewId().ToString();
            mys.Name = dto.Name;
            mys.Description = dto.Description;
            mys.TotalProduct = dto.TotalProduct;
            mys.Price = dto.Price;
            mys.UrlImage = url;
            mys.Title = dto.Title;
            await _unitOfWork.MysteryBoxRepository.AddAsync(mys);

            var box = new MangaBox();
            box.Status = 1;
            box.CreatedAt = DateTime.Now;
            box.UpdatedAt = DateTime.Now;
            box.MysteryBoxId = mys.Id;
            box.CollectionTopicId = dto.CollectionTopicId;
            box.Title = dto.Title;
            await _unitOfWork.MangaBoxRepository.AddAsync(box);
            await _unitOfWork.SaveChangesAsync();

            return true;

        }

        public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync() => await _unitOfWork.MangaBoxRepository.GetAllWithDetailsAsync();
        
        public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id) => await _unitOfWork.MangaBoxRepository.GetByIdWithDetailsAsync(id);

        public async Task<string> BuyBoxAsync(string userId, string boxId, int quantity)
        {
            if (quantity <= 0) throw new Exception("Quantity must be greater than zero");

            var mangaBox = await _unitOfWork.MangaBoxRepository.FindOneAsync(x => x.Id == boxId) ?? throw new Exception("Box not found");
            var mysteryBox = await _unitOfWork.MysteryBoxRepository.FindOneAsync(x => x.Id == mangaBox.MysteryBoxId) ?? throw new Exception("MysteryBox not found");
            int totalPrice = mysteryBox.Price * quantity;
            var user = await _unitOfWork.UserRepository.FindOneAsync(x => x.Id == userId) ?? throw new Exception("User not found");
            string walletId = user.WalletId;
            if (string.IsNullOrEmpty(walletId))
            {
                var newWallet = new UseDigitalWallet
                {
                    Ammount = 0,
                    IsActive = true
                };
                await _unitOfWork.UseDigitalWalletRepository.AddAsync(newWallet);
                walletId = newWallet.Id;
                user.WalletId = walletId;
                await _unitOfWork.UserRepository.UpdateAsync(user.Id, user);
            }

            var wallet = await _unitOfWork.UseDigitalWalletRepository.FindOneAsync(w => w.Id == walletId);
            if (wallet == null || wallet.Ammount < totalPrice) throw new Exception("Insufficient balance");
            
            wallet.Ammount -= totalPrice;
            await _unitOfWork.UseDigitalWalletRepository.UpdateAsync(wallet.Id, wallet);

            var boxOrder = new BoxOrder
            {
                UserId = userId,
                BoxId = boxId,
                Quantity = quantity,
                Amount = totalPrice
            };
            await _unitOfWork.BoxOrderRepository.AddAsync(boxOrder);

            var orderHistory = new OrderHistory
            {
                BoxOrderId = boxOrder.Id,
                ProductOrderId = null,
                Datetime = DateTime.UtcNow,
                Status = (int)TransactionStatus.Success
            };
            await _unitOfWork.OrderHistoryRepository.AddAsync(orderHistory);

            var paymentSession = new DigitalPaymentSession
            {
                WalletId = walletId,
                OrderId = orderHistory.Id,
                Type = DigitalPaymentSessionType.MysteryBox.ToString(),
                Amount = totalPrice,
                IsWithdraw = false
            };
            await _unitOfWork.DigitalPaymentSessionRepository.AddAsync(paymentSession);

            var userBox = await _unitOfWork.UserBoxRepository.FindOneAsync(x => x.UserId == userId && x.BoxId == boxId);
            if (userBox == null)
            {
                var newUserBox = new UserBox
                {
                    UserId = userId,
                    BoxId = boxId,
                    Quantity = quantity,
                    UpdatedAt = DateTime.UtcNow
                };
                await _unitOfWork.UserBoxRepository.AddAsync(newUserBox);
            }
            else
            {
                userBox.Quantity += quantity;
                userBox.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.UserBoxRepository.UpdateAsync(userBox.Id, userBox);
            }

            await _unitOfWork.SaveChangesAsync();

            return orderHistory.Id;
        }
    }
}
