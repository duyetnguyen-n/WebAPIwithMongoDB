using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;

namespace WebAPIwithMongoDB.Repositories.Interface
{
    public interface ILogRepository 
    {
        Task<IEnumerable<Log>> GetAsync();
        Task<Log> GetAsync(string id);
        Task CreateAsync(Log obj);
        Task DeleteAsync(string id);
        Task DeleteAllAsync();
        Task DeleteMultipleAsync(List<string> ids);
        Task<IEnumerable<Log>> GetLogByUserId(string rankId);

    }
}