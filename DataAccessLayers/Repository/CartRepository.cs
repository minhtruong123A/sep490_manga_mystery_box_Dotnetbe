using BusinessObjects.Mongodb;
using BusinessObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly IMongoCollection<Cart> _cartCollection;
        private readonly IMongoCollection<CartProduct> _cartProductCollection;
        private readonly IMongoCollection<CartBox> _cartBoxCollection;
        private readonly IMongoCollection<SellProduct> _sellProductCollection;
        private readonly IMongoCollection<MangaBox> _mangaBoxCollection;

        public CartRepository(MongoDbContext context) : base(context.GetCollection<Cart>("Cart"))
        {
            _cartCollection = context.GetCollection<Cart>("Cart");
            _cartProductCollection = context.GetCollection<CartProduct>("CartProduct");
            _cartBoxCollection = context.GetCollection<CartBox>("CartBox");
            _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
        }

        public async Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.");
            if (string.IsNullOrWhiteSpace(sellProductId) && string.IsNullOrWhiteSpace(mangaBoxId)) 
                throw new ArgumentException("Invalid syntax when adding to cart: you must enter SellProductId or MangaBoxId.");

            var cart = await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };
                await _cartCollection.InsertOneAsync(cart);
            }

            if (!string.IsNullOrWhiteSpace(sellProductId))
            {
                var sellProduct = await _sellProductCollection.Find(c => c.Id == sellProductId).FirstOrDefaultAsync();
                if (sellProduct == null) throw new ArgumentException("SellProduct not existed in system!");
                if (sellProduct.Quantity == 0) throw new ArgumentException("Out of sold!");
                var existingCartProduct = await _cartProductCollection
                                                .Find(c => c.CartId == cart.Id && c.SellProductId == sellProductId)
                                                .FirstOrDefaultAsync();
                if (existingCartProduct != null)
                {
                    var update = Builders<CartProduct>.Update.Inc(c => c.Quantity, 1);
                    await _cartProductCollection.UpdateOneAsync(c => c.Id == existingCartProduct.Id, update);
                }
                else
                {
                    var cartProduct = new CartProduct
                    {
                        CartId = cart.Id,
                        SellProductId = sellProductId,
                        Quantity = 1
                    };
                    await _cartProductCollection.InsertOneAsync(cartProduct);
                }
            }

            if (!string.IsNullOrWhiteSpace(mangaBoxId))
            {
                var mangaBox = await _mangaBoxCollection.Find(c => c.Id == mangaBoxId).FirstOrDefaultAsync();
                if (mangaBox == null) throw new ArgumentException("MangaBox not existed in system!");
                var existingCartBox = await  _cartBoxCollection
                                               .Find(c => c.CartId == cart.Id && c.MangaBoxId == mangaBoxId)
                                               .FirstOrDefaultAsync();
                if (existingCartBox != null)
                {
                    var update = Builders<CartBox>.Update.Inc(c => c.Quantity, 1);
                    await _cartBoxCollection.UpdateOneAsync(c => c.Id == existingCartBox.Id, update);
                }
                else
                {
                    var cartBox = new CartBox
                    {
                        CartId = cart.Id,
                        MangaBoxId = mangaBoxId,
                        Quantity = 1
                    };
                    await _cartBoxCollection.InsertOneAsync(cartBox);
                }
            }
        }

        public async Task RemoveFromCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null)
        {
            var cart = await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
            if (cart == null) throw new ArgumentException("Cart not found.");

            if (!string.IsNullOrWhiteSpace(sellProductId))
            {
                await _cartProductCollection.DeleteOneAsync(p => p.CartId == cart.Id && p.SellProductId == sellProductId);
            }

            if (!string.IsNullOrWhiteSpace(mangaBoxId))
            {
                await _cartBoxCollection.DeleteOneAsync(b => b.CartId == cart.Id && b.MangaBoxId == mangaBoxId);
            }
        }

        public async Task ClearCartAsync(string userId, string type)
        {
            var cart = await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
            if (cart == null) return;

            switch (type.ToLower())
            {
                case "product":
                    await _cartProductCollection.DeleteManyAsync(p => p.CartId == cart.Id);
                    break;
                case "box":
                    await _cartBoxCollection.DeleteManyAsync(b => b.CartId == cart.Id);
                    break;
                case "all":
                    await _cartProductCollection.DeleteManyAsync(p => p.CartId == cart.Id);
                    await _cartBoxCollection.DeleteManyAsync(b => b.CartId == cart.Id);
                    break;
                default:
                    throw new ArgumentException("Invalid cart clear type. Must be 'product', 'box', or 'all'.");
            }
        }

        public async Task<Cart?> GetCartByUserIdAsync(string userId) => await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();

        public async Task<List<CartProduct>> GetCartProductsByCartIdAsync(string cartId) => await _cartProductCollection.Find(c => c.CartId == cartId).ToListAsync();

        public async Task<List<CartBox>> GetCartBoxesByCartIdAsync(string cartId) => await _cartBoxCollection.Find(c => c.CartId == cartId).ToListAsync();

        public async Task<bool> UpdateItemQuantityAsync(string cartId, string itemId, int quantity)
        {
            if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.");

            var cartProduct = await _cartProductCollection.Find(p => p.CartId == cartId && p.SellProductId == itemId).FirstOrDefaultAsync();
            if (cartProduct != null)
            {
                var sellProduct = await _sellProductCollection.Find(sp => sp.Id == cartProduct.SellProductId).FirstOrDefaultAsync();
                if (sellProduct == null) throw new ArgumentException("Product does not exist anymore.");
                if (quantity > sellProduct.Quantity) throw new InvalidOperationException($"Not enough stock for product. Available: {sellProduct.Quantity}, Requested: {quantity}.");
                if (quantity == 0)
                {
                    await _cartProductCollection.DeleteOneAsync(p => p.Id == cartProduct.Id);
                }
                else
                {
                    var update = Builders<CartProduct>.Update.Set(p => p.Quantity, quantity);
                    await _cartProductCollection.UpdateOneAsync(p => p.Id == cartProduct.Id, update);
                }
                return true;
            }

            var cartBox = await _cartBoxCollection.Find(b => b.CartId == cartId && b.MangaBoxId == itemId).FirstOrDefaultAsync();
            if (cartBox != null)
            {
                if (quantity == 0)
                {
                    await _cartBoxCollection.DeleteOneAsync(b => b.Id == cartBox.Id);
                }
                else
                {
                    var update = Builders<CartBox>.Update.Set(b => b.Quantity, quantity);
                    await _cartBoxCollection.UpdateOneAsync(b => b.Id == cartBox.Id, update);
                }
                return true;
            }

            return false;
        }
    }
}
