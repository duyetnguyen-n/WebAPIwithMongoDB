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
    public class CriteriaRepository : BaseRepository<Criteria>, ICriteriaRepository
    {
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;
        private readonly ICriteriaOfAEvaluationRepository _evaluateOfAEvaluationRepository;


        public CriteriaRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ICriteriaOfAEvaluationRepository evaluateOfAEvaluationRepository, ILogRepository auditLogRepository, ICriteriaGroupRepository criteriaGroupRepository) : base(mongoDbContext,httpContextAccessor, auditLogRepository)
        {
            _criteriaGroupRepository = criteriaGroupRepository;
            _evaluateOfAEvaluationRepository = evaluateOfAEvaluationRepository;
        }

        public override async Task CreateAsync(Criteria criteria)
        {
            ArgumentNullException.ThrowIfNull(criteria, nameof(criteria));
            await base.CreateAsync(criteria);

            if (!string.IsNullOrEmpty(criteria.CriteriaGroupId))
            {
                await _criteriaGroupRepository.IncrementCriteriaGroupCount(criteria.CriteriaGroupId);
            }
        }

        public async Task<IEnumerable<Criteria>> GetCriteriesByCriteriaGroupId(string criteriaGroupId){
            var filter = Builders<Criteria>.Filter.Eq(e => e.CriteriaGroupId, criteriaGroupId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public override async Task DeleteAsync(string id)
        {
            var criteria = await GetAsync(id);
            if (criteria == null)
                throw new Exception("criteria not found");

            var evaluations = await _evaluateOfAEvaluationRepository.GetEvaluationesByCriteriaId(id);
            if (evaluations.Any())
            {
                throw new Exception("Không thể xóa vì tiêu chí này đã được sử dụng!");
            }

            await base.DeleteAsync(id);

            if (!string.IsNullOrEmpty(criteria.CriteriaGroupId))
            {
                await _criteriaGroupRepository.DecrementCriteriaGroupCount(criteria.CriteriaGroupId);
            }
        }
    }
}