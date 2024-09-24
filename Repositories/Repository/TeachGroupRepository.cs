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
    public class TeachGroupRepository : BaseRepository<TeachGroup>,ITeachGroupRepository
    {
        public TeachGroupRepository(IMongoDbContext mongoDbContext, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository)
        {
        }
        public async Task IncrementTeachGroupCount(string teachGroupId)
        {
            var filter = Builders<TeachGroup>.Filter.Eq("_id", new ObjectId(teachGroupId));
            var update = Builders<TeachGroup>.Update.Inc(tg => tg.Count, 1);
            await _dbCollection.UpdateOneAsync(filter, update);
        }

        public async Task DecrementTeachGroupCount(string teachGroupId)
        {
            var filter = Builders<TeachGroup>.Filter.Eq("_id", new ObjectId(teachGroupId));
            var update = Builders<TeachGroup>.Update.Inc(tg => tg.Count, -1);
            await _dbCollection.UpdateOneAsync(filter, update);
        }

        public override async Task DeleteAsync(string id){
            var teachGroup = await GetAsync(id);
            if (teachGroup == null)
                throw new Exception("Group not found");

            await base.DeleteAsync(id);
        }


    }
}