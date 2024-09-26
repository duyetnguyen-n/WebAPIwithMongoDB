using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPIwithMongoDB.Services
{
    public interface ICriteriaGroupRoleService
    {
        Task UpdatePointsWhenCriteriaGroupRoleChanges(string criteriaGroupId);
    }
}