using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class UseDigitalWalletRepository(MongoDbContext context)
    : GenericRepository<UseDigitalWallet>(context.GetCollection<UseDigitalWallet>("UseDigitalWallet")),
        IUseDigitalWalletRepository;