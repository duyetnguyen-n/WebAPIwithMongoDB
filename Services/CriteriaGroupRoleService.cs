using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Services
{
    public class CriteriaGroupRoleService : ICriteriaGroupRoleService
    {
        private readonly ICriteriaOfAEvaluationRepository _criteriaOfAEvaluationRepository;
        private readonly IEvaluateRepository _evaluationRepository;
        private readonly IRankRepository _rankRepository;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;
        private readonly IUserRepository _userRepository;

        public CriteriaGroupRoleService(
            ICriteriaOfAEvaluationRepository criteriaOfAEvaluationRepository,
            IEvaluateRepository evaluationRepository,
            IRankRepository rankRepository,
            ICriteriaRepository criteriaRepository,
            ICriteriaGroupRepository criteriaGroupRepository,
            IUserRepository userRepository)
        {
            _criteriaOfAEvaluationRepository = criteriaOfAEvaluationRepository;
            _evaluationRepository = evaluationRepository;
            _rankRepository = rankRepository;
            _criteriaRepository = criteriaRepository;
            _criteriaGroupRepository = criteriaGroupRepository;
            _userRepository = userRepository;
        }

        public async Task UpdatePointsWhenCriteriaGroupRoleChanges(string criteriaGroupId)
        {
            var criteriaGroup = await _criteriaGroupRepository.GetAsync(criteriaGroupId);
            if (criteriaGroup == null) throw new Exception("Criteria group not found.");

            var criterias = await _criteriaRepository.GetCriteriesByCriteriaGroupId(criteriaGroupId);
            foreach (var criteria in criterias)
            {
                var CriteriaOfAEvaluations = await _criteriaOfAEvaluationRepository.GetEvaluationesByCriteriaId(criteria.Id);
                foreach (var data in CriteriaOfAEvaluations)
                {
                    var evaluate = await _evaluationRepository.GetAsync(data.EvaluateId);
                    if (evaluate == null) throw new Exception("Evaluate not found.");

                    if (criteriaGroup.Role.ToLower() == "trừ điểm")
                    {
                        evaluate.TotalPointAddition -= Convert.ToInt32(data.Total);
                        evaluate.TotalPointSubstraction += Convert.ToInt32(data.Total); 
                    }
                    else
                    {
                        evaluate.TotalPointSubstraction -= Convert.ToInt32(data.Total); 
                        evaluate.TotalPointAddition += Convert.ToInt32(data.Total);
                    }

                    var totalOld = evaluate.TotalPoint; 
                    evaluate.TotalPoint = evaluate.TotalPointAddition - evaluate.TotalPointSubstraction;

                    var ranks = await _rankRepository.GetAsync();
                    var rankId = ranks.FirstOrDefault(rank =>
                        Convert.ToInt32(evaluate.TotalPoint) >= Convert.ToInt32(rank.PointRangeStart) &&
                        Convert.ToInt32(evaluate.TotalPoint) <= Convert.ToInt32(rank.PointRangeEnd))?.Id;

                    evaluate.RankId = rankId;

                    await _evaluationRepository.UpdateAsync(evaluate.Id, evaluate);

                    var user = await _userRepository.GetAsync(evaluate.UserId);
                    user.Point = user.Point - totalOld + evaluate.TotalPoint; 
                    await _userRepository.UpdateAsync(user.Id, user);
                }
            }
        }

    }
}