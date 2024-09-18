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
    public class PermistionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermistionRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
        {
            
        }
    }
}