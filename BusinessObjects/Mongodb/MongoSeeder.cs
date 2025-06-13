using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessObjects.Mongodb
{
    public class MongoSeeder
    {
        private readonly MongoDbContext _context;

        public MongoSeeder(MongoDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            var now = DateTime.UtcNow;

            // 1. Insert Rarities
            var rarities = new List<Rarity>
            {
                new Rarity { Id = ObjectId.GenerateNewId().ToString(), Name = "common", CreatedAt = now, UpdatedAt = now },
                new Rarity { Id = ObjectId.GenerateNewId().ToString(), Name = "uncommon", CreatedAt = now, UpdatedAt = now },
                new Rarity { Id = ObjectId.GenerateNewId().ToString(), Name = "rare", CreatedAt = now, UpdatedAt = now },
                new Rarity { Id = ObjectId.GenerateNewId().ToString(), Name = "epic", CreatedAt = now, UpdatedAt = now },
                new Rarity { Id = ObjectId.GenerateNewId().ToString(), Name = "legendary", CreatedAt = now, UpdatedAt = now }
            };
            var rarityCol = _context.GetCollection<Rarity>("Rarity");
            await rarityCol.InsertManyAsync(rarities);

            // 2. Collection
            var collection = new Collection
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Topic = "Tokyo Ghoul"
            };
            var collectionCol = _context.GetCollection<Collection>("Collection");
            await collectionCol.InsertOneAsync(collection);

            // 3. MysteryBox
            var box = new MysteryBox
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = "Tokyo Ghoul Fan Art & Merchandise Designs",
                Description = "Explore a collection of Tokyo Ghoul-themed designs...",
                Price = 20000,
                TotalProduct = 5
            };
            var boxCol = _context.GetCollection<MysteryBox>("MysteryBox");
            await boxCol.InsertOneAsync(box);

            // 4. Insert Products using temporary SeedProduct
            var tempProductList = new List<SeedProduct>
            {
                new("Ken Kaneki (Kakuja Form) - Dark Art", "common","An artistic rendition of Ken Kaneki in his formidable Kakuja form, featuring a dark and intense aesthetic.\r\nrarity: common"),
                new("Shu Tsukiyama - Stylized Purple Portrait", "common","A stylized portrait of Shu Tsukiyama with prominent purple hues, capturing his distinct character.\r\nrarity: common"),
                new("Ken Kaneki - One-Eyed Ghoul Close-up", "common","A close-up illustration of Ken Kaneki's face, highlighting his singular ghoul eye with vibrant pink and cyan accents.\r\nrarity: common"),
                new("Ken Kaneki - Formal Masked Design", "common","A striking black and white design featuring Ken Kaneki in a suit and his iconic mask, with a split-face effect.\r\nrarity: common"),
                new("Ken Kaneki - \"TOKYO\" Grunge Poster", "uncommon","A grunge-style poster featuring white-haired Ken Kaneki against a black background with bold red \"TOKYO\" lettering.\r\nrarity: uncommon"),
                new("Ghoul Eyes - Abstract Panel Art", "uncommon","An abstract design composed of multiple panels showcasing piercing red ghoul eyes and dark, somber elements.\r\nrarity: uncommon"),
                new("Ken Kaneki - Japanese Silhouette Art", "uncommon","A stark silhouette of Ken Kaneki with Japanese text for \"Tokyo Ghoul\" in red on a black background.\r\nrarity: uncommon"),
                new("Ken Kaneki - White Hair Transformation Art", "rare","An illustration of Ken Kaneki with his distinctive white hair, accompanied by Japanese text for \"Tokyo Ghoul\".\r\nrarity: rare"),
                new("Tokyo Ghoul - Ken Kaneki Split Identity", "rare","A dynamic design depicting Ken Kaneki's split identity between his human and ghoul forms, with \"Tokyo Ghoul\" text and a blue border.\r\nrarity: rare"),
                new("Juuzou Suzuya - Character Art", "epic","A full-body character illustration of Juuzou Suzuya in his unique style, with relevant Japanese text and a purple border.\r\nrarity: epic"),
                new("Ken Kaneki - The One-Eyed King", "legendary", "Witness the final metamorphosis of Ken Kaneki in this breathtaking masterpiece. Clad in his iconic black ghoul suit, his Rinkaku Kagune flares like a storm of vengeance beneath a blood-red sky. This ultra-rare card immortalizes the moment he fully embraces his identity as the One-Eyed King—half human, half ghoul, entirely unstoppable.\r\nrarity: legendary")
            };

            var productList = new List<Product>();
            foreach (var temp in tempProductList)
            {
                var rarityId = rarities.First(r => r.Name == temp.RarityName).Id;
                productList.Add(new Product(temp.Title, rarityId, temp.Description)
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    CreatedAt = now,
                    UpdatedAt = now
                });
            }

            var productCol = _context.GetCollection<Product>("Product");
            await productCol.InsertManyAsync(productList);

            // 5. UserBoxes
            var userId = "68443bd125e473716186b92d";
            var userBoxes = new List<UserBox>
            {
                new UserBox { Id = ObjectId.GenerateNewId().ToString(), UserId = userId, BoxId = box.Id, Quantity = 1 },
                new UserBox { Id = ObjectId.GenerateNewId().ToString(), UserId = userId, BoxId = box.Id, Quantity = 0 }
            };
            var userBoxCol = _context.GetCollection<UserBox>("UserBox");
            await userBoxCol.InsertManyAsync(userBoxes);

            // 6. UserProduct
            var userProductCol = _context.GetCollection<UserProduct>("User_Product");
            var openedIndexes = new[] { 0, 3, 7, 9, 10 };
            var userProducts = new List<UserProduct>();
            foreach (var index in openedIndexes)
            {
                userProducts.Add(new UserProduct
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    ProductId = productList[index].Id,
                    CollectionId = collection.Id,
                    Quantity = 1,
                    CollectedAt = now
                    // Not setting UserId because it's not in your model
                });
            }
            await userProductCol.InsertManyAsync(userProducts);

            Console.WriteLine("Seeding completed.");
        }

        // Temporary class for seeding only
        private class SeedProduct
        {
            public string Title { get; set; }
            public string RarityName { get; set; }
            public string Description { get; set; }

            public SeedProduct(string title, string rarity, string description = "")
            {
                Title = title;
                RarityName = rarity;
                Description = string.IsNullOrEmpty(description) ? $"Design of {title}" : description;
            }
        }
    }
}
