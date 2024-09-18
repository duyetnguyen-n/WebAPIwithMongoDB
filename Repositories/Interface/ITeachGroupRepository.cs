using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface ITeachGroupRepository : IBaseRepository<TeachGroup>
    {
        Task IncrementTeachGroupCount(string teachGroupId);
        Task DecrementTeachGroupCount(string teachGroupId);
    }
}