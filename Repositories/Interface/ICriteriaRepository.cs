using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface ICriteriaRepository : IBaseRepository<Criteria>
    {
        Task<IEnumerable<Criteria>> GetCriteriesByCriteriaGroupId(string criteriaGroupId);
    }
}