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
        private readonly IEvaluateRepository _evaluateRepository;
        public RankRepository(IMongoDbContext mongoDbContext, IEvaluateRepository evaluateRepository, ILogRepository auditLogRepository) : base(mongoDbContext, auditLogRepository)
        {
            _evaluateRepository = evaluateRepository; 
        }
        public override async Task DeleteAsync(string id)
        {
            var rank = await GetAsync(id);
            if (rank == null)
                throw new Exception("Rank not found");

            var evaluations = await _evaluateRepository.GetEvaluationsByRankId(id);
            if (evaluations.Any())
            {
                throw new Exception("Không thể xóa được hạng này vì còn đánh giá liên quan!");
            }

            await base.DeleteAsync(id);
        }

    }
}