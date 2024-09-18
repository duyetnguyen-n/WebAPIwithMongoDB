using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class CriteriaOfGroupRepository : BaseRepository<CriteriaOfGroup>, ICriteriaOfGroupRepository
    {
        public CriteriaOfGroupRepository(IMongoDbContext mongoDbContext, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository)
        {

        }
    }
}