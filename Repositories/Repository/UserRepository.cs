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
        private readonly IUserRepository _users; 

        public UserRepository(IMongoDbContext mongoContext, ITeachGroupRepository teachGroupRepository, IEvaluateRepository evaluateRepository, ILogRepository auditLogRepository)
            : base(mongoContext, auditLogRepository)
        {
            _teachGroupRepository = teachGroupRepository;
            _evaluateRepository = evaluateRepository;
        }

        public async Task<IEnumerable<User>> GetUsersByTeachGroupId(string teachGroupId)
        {
            var filter = Builders<User>.Filter.Eq(e => e.TeachGroupId, teachGroupId);
            return await _dbCollection.Find(filter).ToListAsync();
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

            var evaluations = await _evaluateRepository.GetEvaluationsByUserId(id);
            if (evaluations.Any())
            {
                foreach (Evaluate item in evaluations){
                   await _evaluateRepository.DeleteAsync(item.Id);
                }
            }

            await base.DeleteAsync(id);

            if (!string.IsNullOrEmpty(user.TeachGroupId))
            {
                await _teachGroupRepository.DecrementTeachGroupCount(user.TeachGroupId);
            }
        }
        public async Task<User> FindByNumberPhoneAsync(string numberPhone)
        {
            var filter = Builders<User>.Filter.Eq(u => u.NumberPhone, numberPhone);
            return await _dbCollection.Find(filter).FirstOrDefaultAsync();
        }

    }
}
