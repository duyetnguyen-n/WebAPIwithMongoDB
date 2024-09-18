using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class EvaluateRepository : BaseRepository<Evaluate>, IEvaluateRepository
    {
        public EvaluateRepository(IMongoDbContext mongoDbContext) : base(mongoDbContext)
        {
            
        }
        public async Task<IEnumerable<Evaluate>> GetEvaluationsByUserId(string userId)
        {
            var filter = Builders<Evaluate>.Filter.Eq(e => e.UserId, userId);
            return await _dbCollection.Find(filter).ToListAsync();
        }
    }
}