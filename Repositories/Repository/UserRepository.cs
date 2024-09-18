using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        private readonly ITeachGroupRepository _teachGroupRepository;
        private readonly IEvaluateRepository _evaluateRepository; 

        public UserRepository(IMongoDbContext mongoContext, ITeachGroupRepository teachGroupRepository, IEvaluateRepository evaluateRepository, ILogRepository auditLogRepository)
            : base(mongoContext, auditLogRepository)
        {
            _teachGroupRepository = teachGroupRepository;
            _evaluateRepository = evaluateRepository;
        }


        public override async Task CreateAsync(User user)
        {
            ArgumentNullException.ThrowIfNull(user, nameof(user));
            await base.CreateAsync(user);

            if (!string.IsNullOrEmpty(user.TeachGroupId))
            {
                await _teachGroupRepository.IncrementTeachGroupCount(user.TeachGroupId);
            }
        }

        public override async Task DeleteAsync(string id)
        {
            var user = await GetAsync(id);
            if (user == null)
                throw new Exception("User not found");

            // var evaluations = await _evaluateRepository.GetEvaluationsByUserId(id);
            // if (evaluations.Any())
            // {
            //     throw new Exception("User has evaluations. Please handle evaluations before deleting.");
            // }

            await base.DeleteAsync(id);

            if (!string.IsNullOrEmpty(user.TeachGroupId))
            {
                await _teachGroupRepository.DecrementTeachGroupCount(user.TeachGroupId);
            }
        }
    }
}
