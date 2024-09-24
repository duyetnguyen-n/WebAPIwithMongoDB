using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface ICriteriaOfAEvaluationRepository : IBaseRepository<CriteriaOfAEvaluation>
    {
        Task<IEnumerable<CriteriaOfAEvaluation>> GetEvaluationesByCriteriaId(string criteriaId);

    }
}