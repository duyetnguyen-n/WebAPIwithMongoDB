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
    public class RankRepository : BaseRepository<Rank>, IRankRepository
    {
        public RankRepository(IMongoDbContext mongoDbContext, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository)
        {

        }
    }
}