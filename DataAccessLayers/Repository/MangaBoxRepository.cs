using BusinessObjects;
using BusinessObjects.Dtos.MangaBox;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;
using DataAccessLayers.Pipelines;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayers.Repository
{
    public class MangaBoxRepository : GenericRepository<MangaBox>, IMangaBoxRepository
    {
        private readonly IMongoCollection<MangaBox> _mangaBoxCollection;
        private readonly IMongoCollection<MysteryBox> _mysteryBoxCollection;
        private readonly IMongoCollection<Collection> _collectionCollection;
        public MangaBoxRepository(MongoDbContext context) : base(context.GetCollection<MangaBox>("MangaBox"))
        {
            _mangaBoxCollection = context.GetCollection<MangaBox>("MangaBox");
            _mysteryBoxCollection = context.GetCollection<MysteryBox>("MysteryBox");
            _collectionCollection = context.GetCollection<Collection>("Collection");
        }

        //public async Task<List<MangaBoxDetailDto>> GetAllWithDetailsAsync()
        //{
        //    var results = await _mangaBoxCollection.Aggregate()
        //        .AppendStage<BsonDocument>(new BsonDocument("$addFields", new BsonDocument
        //        {
        //            { "MysteryBoxId", new BsonDocument("$toObjectId", new BsonDocument("$trim", new BsonDocument("input", "$MysteryBoxId"))) },
        //            { "CollectionTopicId", new BsonDocument("$toObjectId", new BsonDocument("$trim", new BsonDocument("input", "$CollectionTopicId"))) }
        //        }))
        //        .Lookup("MysteryBox", "MysteryBoxId", "_id", "MysteryBox")
        //        .Lookup("Collection", "CollectionTopicId", "_id", "Collection")
        //        .Unwind("MysteryBox")
        //        .Unwind("Collection")
        //        .Project(Builders<BsonDocument>.Projection
        //            .Include("Status")
        //            .Include("MysteryBox.Name")
        //            .Include("MysteryBox.Description")
        //            .Include("MysteryBox.Price")
        //            .Include("Collection.Topic")
        //            .Include("_id"))
        //        .ToListAsync();

        //    return results.Select(x => new MangaBoxDetailDto
        //    {
        //        Id = x.GetValue("_id").ToString(),
        //        Status = x.GetValue("Status").ToInt32(),
        //        MysteryBoxName = x["MysteryBox"]["Name"].AsString,
        //        MysteryBoxDescription = x["MysteryBox"]["Description"].AsString,
        //        MysteryBoxPrice = x["MysteryBox"]["Price"].ToInt32(),
        //        CollectionTopic = x["Collection"]["Topic"].AsString
        //    }).ToList();
        //}

        public async Task<List<MangaBoxGetAllDto>> GetAllWithDetailsAsync()
        {
            var results = await _mangaBoxCollection
                .WithBson()
                .RunAggregateWithLookups(
                    MangaBoxPipelineBuilder.BuildAllPipeline,
                    x => new MangaBoxGetAllDto
                    {
                        Id = x.GetValue("_id").ToString(),
                        MysteryBoxName = x["MysteryBox"]["Name"].AsString,
                        MysteryBoxPrice = x["MysteryBox"]["Price"].ToInt32(),
                        UrlImage = x["MysteryBox"]["UrlImage"].AsString,
                        CollectionTopic = x["Collection"]["Topic"].AsString
                    });

            return results;
        }

        public async Task<MangaBoxDetailDto?> GetByIdWithDetailsAsync(string id)
        {
            var objectId = ObjectId.Parse(id);

            return await _mangaBoxCollection
                .WithBson()
                .RunAggregateWithLookupsSingle(
                    pipeline => MangaBoxPipelineBuilder.BuildDetailPipeline(pipeline, objectId),
                    x => new MangaBoxDetailDto
                    {
                        Id = x.GetValue("_id").ToString(),
                        Status = x.GetValue("Status").ToInt32(),
                        MysteryBoxName = x["MysteryBox"]["Name"].AsString,
                        MysteryBoxDescription = x["MysteryBox"]["Description"].AsString,
                        MysteryBoxPrice = x["MysteryBox"]["Price"].ToInt32(),
                        UrlImage = x["MysteryBox"]["UrlImage"].AsString,
                        CollectionTopic = x["Collection"]["Topic"].AsString
                    });
        }
    }
}
