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
        public EvaluateRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ILogRepository auditLogRepository) : base(mongoDbContext, httpContextAccessor, auditLogRepository)
        {
            
        }
        public async Task<IEnumerable<Evaluate>> GetEvaluationsByUserId(string userId)
        {
            var filter = Builders<Evaluate>.Filter.Eq(e => e.UserId, userId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Evaluate>> GetEvaluationsByRankId(string rankId)
        {
            var filter = Builders<Evaluate>.Filter.Eq(e => e.RankId, rankId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

        // public async Task<IEnumerable<Evaluate>> GetEvaluationsByCriteriaId(string criteriaId)
        // {
        //     var filter = Builders<Evaluate>.Filter.Eq(e => e.CriteriaId, criteriaId);
        //     return await _dbCollection.Find(filter).ToListAsync();
        // }
    }
}