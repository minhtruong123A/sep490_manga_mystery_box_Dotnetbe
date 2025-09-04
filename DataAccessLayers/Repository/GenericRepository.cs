using System.Linq.Expressions;
using DataAccessLayers.Interface;
using MongoDB.Driver;

namespace DataAccessLayers.Repository;

public class GenericRepository<T>(IMongoCollection<T> collection) : IGenericRepository<T>
    where T : class

{
    protected readonly IMongoCollection<T> _collection = collection;

    public async Task<T?> GetByIdAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
    }

    public async Task<T?> FindOneAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.Find(expression).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.Find(expression).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _collection.InsertManyAsync(entities);
        return entities;
    }

    public async Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task DeleteRangeAsync(Expression<Func<T, bool>> expression)
    {
        await _collection.DeleteManyAsync(expression);
    }

    public async Task UpdateAsync(string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.ReplaceOneAsync(filter, entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.Find(expression).AnyAsync();
    }

    public async Task<T?> GetOneAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.Find(expression).FirstOrDefaultAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _collection.DeleteManyAsync(Builders<T>.Filter.Empty);
    }

    public async Task UpdateFieldAsync(Expression<Func<T, bool>> filter, UpdateDefinition<T> update)
    {
        await _collection.UpdateOneAsync(filter, update);
    }

    public async Task<List<T>> FilterByAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }


    public async Task<T?> FindOneAsync(IClientSessionHandle session, Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(session, predicate).FirstOrDefaultAsync();
    }

    public async Task AddAsync(IClientSessionHandle session, T entity)
    {
        await _collection.InsertOneAsync(session, entity);
    }

    public async Task<T?> GetByIdAsync(IClientSessionHandle session, string id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(session, filter).FirstOrDefaultAsync();
    }

    public async Task UpdateAsync(IClientSessionHandle session, string id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        await _collection.ReplaceOneAsync(session, filter, entity);
    }

    public async Task UpdateFieldAsync(IClientSessionHandle session, Expression<Func<T, bool>> filter,
        UpdateDefinition<T> update)
    {
        await _collection.UpdateOneAsync(session, filter, update);
    }

    public async Task<IEnumerable<T>> FindAllAsync(IClientSessionHandle session, Expression<Func<T, bool>> predicate)
    {
        return await _collection.Find(session, predicate).ToListAsync();
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>> expression)
    {
        return await _collection.CountDocumentsAsync(expression);
    }
}