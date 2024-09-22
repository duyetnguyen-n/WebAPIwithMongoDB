using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface IUserRepository : IBaseRepository<User>
    {
        // Task<IEnumerable<User>> GetUsersByPositionId(string positionId);
        // Task<IEnumerable<User>> GetUsersByTeachGroupId(string teachGroupId);
        Task<User> FindByNumberPhoneAsync(string numberPhone);
    }
}