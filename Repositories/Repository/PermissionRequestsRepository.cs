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
    public class PermissionRequestsRepository : BaseRepository<PermissionRequests>, IPermissionRequestsRepository
    {
        public PermissionRequestsRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ILogRepository auditLogRepository) : base(mongoDbContext, httpContextAccessor, auditLogRepository)
        {

        }
    }
}