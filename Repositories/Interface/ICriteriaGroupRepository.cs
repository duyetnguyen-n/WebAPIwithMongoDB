using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface ICriteriaGroupRepository : IBaseRepository<CriteriaGroup>
    {
        Task IncrementCriteriaGroupCount(string criteriaGroupId);
        Task DecrementCriteriaGroupCount(string criteriaGroupId);
    }
}