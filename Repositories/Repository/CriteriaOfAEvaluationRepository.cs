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
    
    public class CriteriaOfAEvaluationRepository : BaseRepository<CriteriaOfAEvaluation>, ICriteriaOfAEvaluationRepository
    {
        public CriteriaOfAEvaluationRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ILogRepository auditLogRepository) : base(mongoDbContext, httpContextAccessor, auditLogRepository)
        {
            
        }

        public async Task<IEnumerable<CriteriaOfAEvaluation>> GetEvaluationesByCriteriaId(string criteriaId){
            var filter = Builders<CriteriaOfAEvaluation>.Filter.Eq(e => e.CriteriaId, criteriaId);
            return await _dbCollection.Find(filter).ToListAsync();
        }
        public async Task<IEnumerable<CriteriaOfAEvaluation>> GetCriteriesByEvaluateId(string evaluateId)
        {
            var filter = Builders<CriteriaOfAEvaluation>.Filter.Eq(e => e.EvaluateId, evaluateId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

    }
}