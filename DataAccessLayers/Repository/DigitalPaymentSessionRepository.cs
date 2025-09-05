using BusinessObjects;
using BusinessObjects.Mongodb;
using DataAccessLayers.Interface;

namespace DataAccessLayers.Repository;

public class DigitalPaymentSessionRepository(MongoDbContext context)
    : GenericRepository<DigitalPaymentSession>(context.GetCollection<DigitalPaymentSession>("DigitalPaymentSession")),
        IDigitalPaymentSessionRepository;