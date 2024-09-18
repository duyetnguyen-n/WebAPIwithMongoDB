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
    public class CriteriaGroupRepository : BaseRepository<CriteriaGroup>, ICriteriaGroupRepository
    {
        public CriteriaGroupRepository(IMongoDbContext mongoDbContext, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository){}
        public async Task IncrementCriteriaGroupCount(string criteriaGroupId)
        {
            var filter = Builders<CriteriaGroup>.Filter.Eq("_id", new ObjectId(criteriaGroupId));
            var update = Builders<CriteriaGroup>.Update.Inc(tg => tg.Count, 1);
            await _dbCollection.UpdateOneAsync(filter, update);
        }

        public async Task DecrementCriteriaGroupCount(string criteriaGroupId)
        {
            var filter = Builders<CriteriaGroup>.Filter.Eq("_id", new ObjectId(criteriaGroupId));
            var update = Builders<CriteriaGroup>.Update.Inc(tg => tg.Count, -1);
            await _dbCollection.UpdateOneAsync(filter, update);
        }
    }
}