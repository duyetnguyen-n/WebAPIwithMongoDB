using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Entities;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class PositionRepository : BaseRepository<Position>,IPositionRepository
    {
        public PositionRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ILogRepository auditLogRepository) : base(mongoDbContext, httpContextAccessor, auditLogRepository)
        {
        
        }

    }
}