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
    
    public class PermissionOfAPositionRepository : BaseRepository<PermissionOfAPosition>, IPermissionOfAPositionRepository
    {
        public PermissionOfAPositionRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
        {

        }
    }
}