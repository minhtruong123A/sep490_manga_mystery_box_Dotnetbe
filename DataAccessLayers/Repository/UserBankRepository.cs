using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class UserBankRepository(MongoDbContext context)
    : GenericRepository<UserBank>(context.GetCollection<UserBank>("UserBank")), IUserBankRepository;