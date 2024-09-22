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
    
    public class PermissionOfAPositionRepository : BaseRepository<PermissionOfAPosition>, IPermissionOfAPositionRepository
    {
        public PermissionOfAPositionRepository(IMongoDbContext mongoDbContext, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository)
        {
            
        }

        // public async Task<List<PermissionOfAPosition>> FindAllByPositionIdAsync(string id)
        // {
        //     ObjectId objectId = new ObjectId(id);
        //     FilterDefinition<PermissionOfAPosition> filter = Builders<PermissionOfAPosition>.Filter.Eq("positionId", objectId);
        //     IAsyncCursor<PermissionOfAPosition> query = await Query(filter);
        //     return await query.ToListAsync();
        // }
    }
}