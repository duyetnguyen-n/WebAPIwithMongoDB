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
    public class CriteriaRepository : BaseRepository<Criteria>, ICriteriaRepository
    {
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;
        private readonly IEvaluateRepository _evaluateRepository;


        public CriteriaRepository(IMongoDbContext mongoDbContext, IEvaluateRepository evaluateRepository, ILogRepository auditLogRepository, ICriteriaGroupRepository criteriaGroupRepository) : base(mongoDbContext, auditLogRepository)
        {
            _criteriaGroupRepository = criteriaGroupRepository;
            _evaluateRepository = evaluateRepository;
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

        public override async Task DeleteAsync(string id)
        {
            var criteria = await GetAsync(id);
            if (criteria == null)
                throw new Exception("criteria not found");

            // var evaluations = await _evaluateRepository.GetEvaluationsByCriteriaId(id);
            // if (evaluations.Any())
            // {
            //     throw new Exception("criteria has evaluations. Please handle evaluations before deleting.");
            // }

            await base.DeleteAsync(id);

            if (!string.IsNullOrEmpty(criteria.CriteriaGroupId))
            {
                await _criteriaGroupRepository.DecrementCriteriaGroupCount(criteria.CriteriaGroupId);
            }
        }
    }
}