using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface IEvaluateRepository : IBaseRepository<Evaluate>
    {
        Task<IEnumerable<Evaluate>> GetEvaluationsByUserId(string userId);
        Task<IEnumerable<Evaluate>> GetEvaluationsByRankId(string rankId);
    }
}