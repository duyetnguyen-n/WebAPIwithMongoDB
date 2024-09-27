using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class LogRepository : ILogRepository
    {
        protected readonly IMongoDbContext _mongoContext;
        protected IMongoCollection<Log> _dbCollection;
        public LogRepository(IMongoDbContext mongoDbContext) 
        {
            _mongoContext = mongoDbContext;
            _dbCollection = _mongoContext.GetCollection<Log>(typeof(Log).Name);
        }

        public async Task CreateAsync(Log obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            await _dbCollection.InsertOneAsync(obj);
        }

        public async Task DeleteAllAsync()
        {
            var filter = Builders<Log>.Filter.Empty;
            await _dbCollection.DeleteManyAsync(filter);
        }

        public async Task DeleteAsync(string id)
        {
            ObjectId objectId = new ObjectId(id);
            FilterDefinition<Log> filter = Builders<Log>.Filter.Eq("_id", objectId);
            await _dbCollection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<Log>> GetAsync()
        {
            IAsyncCursor<Log> query = await Query(Builders<Log>.Filter.Empty);
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Log>> GetLogByUserId(string userId){
            var filter = Builders<Log>.Filter.Eq(e => e.UserId, userId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<Log> GetAsync(string id)
        {
            ObjectId objectId = new ObjectId(id);
            FilterDefinition<Log> filter = Builders<Log>.Filter.Eq("_id", objectId);
            IAsyncCursor<Log> query = await Query(filter);
            return await query.FirstOrDefaultAsync();
        }
        protected async Task<IAsyncCursor<Log>> Query(FilterDefinition<Log> filter)
        {
            return await _dbCollection.FindAsync<Log>(filter);
        }

        public async Task DeleteMultipleAsync(List<string> ids)
        {
            var objectIds = ids.Select(id => new ObjectId(id)).ToList();
            var filter = Builders<Log>.Filter.In("_id", objectIds);

            await _dbCollection.DeleteManyAsync(filter);

            // // Thêm một log để ghi lại hành động bulk delete
            // var log = new Log
            // {
            //     UserId = "66ea2c41933b21975a8fb8d1",
            //     Action = "DeleteMultiple",
            //     TimeStamp = DateTime.UtcNow,
            //     Status = "available",
            //     Description = $"admin đã xóa nhiều log với các id: {string.Join(", ", ids)}"
            // };

            // await _logs.InsertOneAsync(log);
        }

    }
}