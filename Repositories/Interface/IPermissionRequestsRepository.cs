using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface IPermissionRequestsRepository : IBaseRepository<PermissionRequests>
    {
        // Task<User> getUserByPermissionRequestsId(string permissionRequestsId);
        // Task<Evaluate> getEvaluateByPermissionRequestsId(string permissionRequestsId);
    }
}