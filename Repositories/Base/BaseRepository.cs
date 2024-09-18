using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;

namespace WebAPIwithMongoDB.Repositories.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IMongoEntity
    {
        protected readonly IMongoDbContext _mongoContext;
        protected IMongoCollection<TEntity> _dbCollection;

        protected BaseRepository(IMongoDbContext mongoContext)
        {
            _mongoContext = mongoContext;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            IAsyncCursor<TEntity> query = await Query(Builders<TEntity>.Filter.Empty);

            return await query.ToListAsync();

            // Second Phase

            //var allbooks = await _dbCollection.FindAsync(_ => true).Result.ToListAsync();
        }

        public async Task<TEntity> GetAsync(string id)
        {
            ObjectId objectId = new ObjectId(id);

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            IAsyncCursor<TEntity> query = await Query(filter);

            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task CreateAsync(TEntity obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));

            await _dbCollection.InsertOneAsync(obj);
        }

        public virtual async Task UpdateAsync(string id, TEntity obj)
        {
            ObjectId objectId = new ObjectId(id);

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            await _dbCollection.ReplaceOneAsync(filter, obj);
        }

        public virtual async Task DeleteAsync(string id)
        {
            ObjectId objectId = new ObjectId(id);

            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            await _dbCollection.DeleteOneAsync(filter);

            // Second Phase

            //await _dbCollection.DeleteOneAsync(b => b.Id == id);
        }

        public async Task DeleteAllAsync()
        {
            var filter = Builders<TEntity>.Filter.Empty;

            await _dbCollection.DeleteManyAsync(filter);

            // Second Phase

            //await _dbCollection.DeleteManyAsync(_ => true);
        }

        public async Task<bool> Exists(string id){
            ObjectId objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            var count = await _dbCollection.CountDocumentsAsync(filter);
            return count > 0;
        }

        protected async Task<IAsyncCursor<TEntity>> Query(FilterDefinition<TEntity> filter)
        {
            return await _dbCollection.FindAsync<TEntity>(filter);
        }
    }

}