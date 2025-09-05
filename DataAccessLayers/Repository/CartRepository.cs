using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class CartRepository(MongoDbContext context)
    : GenericRepository<Cart>(context.GetCollection<Cart>("Cart")), ICartRepository
{
    private readonly IMongoCollection<CartBox> _cartBoxCollection = context.GetCollection<CartBox>("CartBox");
    private readonly IMongoCollection<Cart> _cartCollection = context.GetCollection<Cart>("Cart");
    private readonly IMongoCollection<CartProduct> _cartProductCollection = context.GetCollection<CartProduct>("CartProduct");
    private readonly IMongoCollection<MangaBox> _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
    private readonly IMongoCollection<SellProduct> _sellProductCollection = context.GetCollection<SellProduct>("SellProduct");

    public async Task AddToCartAsync(string userId, string? sellProductId = null, string? mangaBoxId = null,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("UserId is required.");
        if (string.IsNullOrWhiteSpace(sellProductId) && string.IsNullOrWhiteSpace(mangaBoxId))
            throw new ArgumentException(
                "Invalid syntax when adding to cart: you must enter SellProductId or MangaBoxId.");
        if (quantity <= 0) quantity = 1;

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
            var currentQuantityInCart = existingCartProduct?.Quantity ?? 0;
            var totalQuantity = currentQuantityInCart + quantity;
            if (totalQuantity > sellProduct.Quantity)
            {
                var availableToAdd = sellProduct.Quantity - currentQuantityInCart;
                throw new ArgumentException(
                    $"Only {sellProduct.Quantity} item(s) in stock. You already have {currentQuantityInCart} in cart. You can add up to {availableToAdd} more.");
            }

            if (existingCartProduct != null)
            {
                var update = Builders<CartProduct>.Update.Inc(c => c.Quantity, quantity);
                await _cartProductCollection.UpdateOneAsync(c => c.Id == existingCartProduct.Id, update);
            }
            else
            {
                var cartProduct = new CartProduct
                {
                    CartId = cart.Id,
                    SellProductId = sellProductId,
                    Quantity = quantity
                };
                await _cartProductCollection.InsertOneAsync(cartProduct);
            }
        }

        if (!string.IsNullOrWhiteSpace(mangaBoxId))
        {
            var mangaBox = await _mangaBoxCollection.Find(c => c.Id == mangaBoxId).FirstOrDefaultAsync();
            if (mangaBox == null) throw new ArgumentException("MangaBox not existed in system!");
            var existingCartBox = await _cartBoxCollection
                .Find(c => c.CartId == cart.Id && c.MangaBoxId == mangaBoxId)
                .FirstOrDefaultAsync();
            if (existingCartBox != null)
            {
                var update = Builders<CartBox>.Update.Inc(c => c.Quantity, quantity);
                await _cartBoxCollection.UpdateOneAsync(c => c.Id == existingCartBox.Id, update);
            }
            else
            {
                var cartBox = new CartBox
                {
                    CartId = cart.Id,
                    MangaBoxId = mangaBoxId,
                    Quantity = quantity
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
            await _cartProductCollection.DeleteOneAsync(p => p.CartId == cart.Id && p.SellProductId == sellProductId);

        if (!string.IsNullOrWhiteSpace(mangaBoxId))
            await _cartBoxCollection.DeleteOneAsync(b => b.CartId == cart.Id && b.MangaBoxId == mangaBoxId);
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

    public async Task<Cart?> GetCartByUserIdAsync(string userId)
    {
        return await _cartCollection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
    }

    public async Task<List<CartProduct>> GetCartProductsByCartIdAsync(string cartId)
    {
        return await _cartProductCollection.Find(c => c.CartId == cartId).ToListAsync();
    }

    public async Task<List<CartBox>> GetCartBoxesByCartIdAsync(string cartId)
    {
        return await _cartBoxCollection.Find(c => c.CartId == cartId).ToListAsync();
    }

    public async Task<bool> UpdateItemQuantityAsync(string cartId, string itemId, int quantity)
    {
        if (quantity < 0) throw new ArgumentException("Quantity cannot be negative.");

        var cartProduct = await _cartProductCollection.Find(p => p.CartId == cartId && p.SellProductId == itemId)
            .FirstOrDefaultAsync();
        if (cartProduct != null)
        {
            var sellProduct = await _sellProductCollection.Find(sp => sp.Id == cartProduct.SellProductId)
                .FirstOrDefaultAsync();
            if (sellProduct == null) throw new ArgumentException("Product does not exist anymore.");
            if (quantity > sellProduct.Quantity)
                throw new InvalidOperationException(
                    $"Not enough stock for product. Available: {sellProduct.Quantity}, Requested: {quantity}.");
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

        var cartBox = await _cartBoxCollection.Find(b => b.CartId == cartId && b.MangaBoxId == itemId)
            .FirstOrDefaultAsync();
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