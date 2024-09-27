using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Base;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Repositories.Repository
{
    public class EvaluateRepository : BaseRepository<Evaluate>, IEvaluateRepository
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogRepository _auditLogRepository;


        public EvaluateRepository(IMongoDbContext mongoDbContext, IHttpContextAccessor httpContextAccessor, ILogRepository auditLogRepository) : base(mongoDbContext, httpContextAccessor, auditLogRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _auditLogRepository = auditLogRepository;
        }
        public async Task<IEnumerable<Evaluate>> GetEvaluationsByUserId(string userId)
        {
            var filter = Builders<Evaluate>.Filter.Eq(e => e.UserId, userId);
            return await _dbCollection.Find(filter).ToListAsync();
        }

        public async Task<IEnumerable<Evaluate>> GetEvaluationsByRankId(string rankId)
        {
            var filter = Builders<Evaluate>.Filter.Eq(e => e.RankId, rankId);
            return await _dbCollection.Find(filter).ToListAsync();
        }
        public async Task UpdateEvaluateSTT(string id, int stt)
        {
            // Chuyển đổi string id sang ObjectId
            ObjectId objectId = new ObjectId(id);

            // Tạo filter để tìm đối tượng Evaluate có _id tương ứng
            var filter = Builders<Evaluate>.Filter.Eq("_id", objectId);

            // Kiểm tra xem đối tượng Evaluate có tồn tại hay không
            var existingEvaluate = await _dbCollection.Find(filter).FirstOrDefaultAsync();

            if (existingEvaluate == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy Evaluate với id: {id}");
            }

            // Tạo cập nhật cho thuộc tính stt
            var update = Builders<Evaluate>.Update.Set(e => e.Stt, stt);

            // Thực hiện cập nhật đối tượng
            await _dbCollection.UpdateOneAsync(filter, update);

            // Ghi log thay đổi
            string userId = null;
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            }

            var log = new Log
            {
                UserId = userId,
                Action = "Update",
                TimeStamp = DateTime.UtcNow,
                Status = "available",
                Description = $"admin đã sửa Evaluate: stt từ '{existingEvaluate.Stt}' thành '{stt}'"
            };

            await _auditLogRepository.CreateAsync(log);
        }

    }
}