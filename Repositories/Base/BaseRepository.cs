using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WebAPIwithMongoDB.Context;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Repositories.Base
{
    public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IMongoEntity
    {
        protected readonly IMongoDbContext _mongoContext;
        protected IMongoCollection<TEntity> _dbCollection;
        private readonly ILogRepository _auditLogRepository;

        protected BaseRepository(IMongoDbContext mongoContext, ILogRepository auditLogRepository)
        {
            _mongoContext = mongoContext;
            _dbCollection = _mongoContext.GetCollection<TEntity>(typeof(TEntity).Name);
            _auditLogRepository = auditLogRepository;
        }

        public async Task<IEnumerable<TEntity>> GetAsync()
        {
            IAsyncCursor<TEntity> query = await Query(Builders<TEntity>.Filter.Empty);
            return await query.ToListAsync();
        }

        public async Task<TEntity> GetAsync(string id)
        {
            ObjectId objectId = new ObjectId(id);
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            IAsyncCursor<TEntity> query = await Query(filter);
            return await query.FirstOrDefaultAsync();
        }

        public virtual async Task CreateAsync(TEntity obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            await _dbCollection.InsertOneAsync(obj);

            var objType = typeof(TEntity);
            var nameProperty = objType.GetProperty("Name");
            string description;
            if (nameProperty != null)
            {
                var nameValue = nameProperty.GetValue(obj)?.ToString();
                description = $"admin đã thêm {nameValue} vào {objType.Name}";
            }
            else
            {
                description = $"admin đã thêm {obj.Id} vào {objType.Name}";
            }
            var log = new Log
            {
                UserId = "66ea2c41933b21975a8fb8d1",
                Action = "Create",
                TimeStamp = DateTime.UtcNow,
                Status = "available",
                Description = description
            };
            await _auditLogRepository.CreateAsync(log);
        }

        public virtual async Task UpdateAsync(string id, TEntity obj)
        {
            ObjectId objectId = new ObjectId(id);
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            var existingCursor = await Query(Builders<TEntity>.Filter.Eq("_id", objectId));
            var existingObj = await existingCursor.FirstOrDefaultAsync();

            if (existingObj == null)
                throw new KeyNotFoundException($"Không tìm thấy đối tượng {id}");

            var objType = typeof(TEntity);
            var changes = new List<string>();

            foreach (var property in objType.GetProperties())
            {
                var oldValue = property.GetValue(existingObj)?.ToString();
                var newValue = property.GetValue(obj)?.ToString();

                if (oldValue != newValue)
                {
                    changes.Add($"{property.Name}: từ '{oldValue}' thành '{newValue}'");
                }
            }

            await _dbCollection.ReplaceOneAsync(filter, obj);

            var description = changes.Any() ? string.Join(", ", changes) : "Không có thay đổi nào";

            var log = new Log
            {
                UserId = "66ea2c41933b21975a8fb8d1", 
                Action = "Update",
                TimeStamp = DateTime.UtcNow,
                Status = "available",
                Description = $"admin đã sửa {typeof(TEntity).Name}: {description}"
            };

            await _auditLogRepository.CreateAsync(log);
        }

        public virtual async Task DeleteAsync(string id)
        {
            var entityType = typeof(TEntity).Name;

            bool isReferenced = await IsReferencedInAnyCollectionAsync(id, entityType);
            Console.WriteLine($"EntityType: {entityType}, ID: {id}, IsReferenced: {isReferenced}"); // Debugging line

            if (isReferenced==false)
            {
                throw new InvalidOperationException("Không thể xóa vì đang có liên kết với các bảng khác.");
            }

            ObjectId objectId = new ObjectId(id);
            FilterDefinition<TEntity> filter = Builders<TEntity>.Filter.Eq("_id", objectId);

            await _dbCollection.DeleteOneAsync(filter);

            var objType = typeof(TEntity);
            var nameProperty = objType.GetProperty("name");

            string description;
            if (nameProperty != null)
            {
                var entity = await _dbCollection.Find(filter).FirstOrDefaultAsync();
                var nameValue = nameProperty.GetValue(entity)?.ToString();
                description = $"admin đã xóa {nameValue} trong {objType.Name}";
            }
            else
            {
                description = $"admin đã xóa đối tượng có id {id} trong {objType.Name}";
            }

            var log = new Log
            {
                UserId = "66ea2c41933b21975a8fb8d1",
                Action = "Delete",
                TimeStamp = DateTime.UtcNow,
                Status = "available",
                Description = description
            };

            await _auditLogRepository.CreateAsync(log);
        }


        public async Task DeleteAllAsync()
        {
            var filter = Builders<TEntity>.Filter.Empty;
            await _dbCollection.DeleteManyAsync(filter);

            var log = new Log
            {
                UserId = "66ea2c41933b21975a8fb8d1",
                Action = "DeleteAll",
                TimeStamp = DateTime.UtcNow,
                Status = "available",
                Description = $"admin đã xóa toàn bộ bảng {typeof(TEntity).Name}"
            };
            await _auditLogRepository.CreateAsync(log);
        }

        public async Task<bool> Exists(string id)
        {
            ObjectId objectId = new ObjectId(id);
            var filter = Builders<TEntity>.Filter.Eq("_id", objectId);
            var count = await _dbCollection.CountDocumentsAsync(filter);
            return count > 0;
        }

        protected async Task<IAsyncCursor<TEntity>> Query(FilterDefinition<TEntity> filter)
        {
            return await _dbCollection.FindAsync<TEntity>(filter);
        }

        private static readonly Dictionary<string, List<string>> ReferenceMapping = new Dictionary<string, List<string>>
        {
            { "User", new List<string> { "Id", "PositionId", "TeachGroupId" } },
            { "Position", new List<string> { "Id", "PersonCheck", "PermissionOfAPositionPositionId" } },
            { "TeachGroup", new List<string> { "Id" } },
            { "PermissionOfAPosition", new List<string> { "PositionId", "PermissionId" } },
            { "Criteria", new List<string> { "PersonCheck", "CriteriaGroupId" } },
            { "CriteriaGroup", new List<string> { "Id" } },
            { "Evaluate", new List<string> { "UserId", "CriteriaId", "RankId" } },
            { "Log", new List<string> { "UserId" } },
            { "PermissionRequests", new List<string> { "UserId", "RequestedPermissionId", "ReviewerId" } }
        };


        public virtual async Task<bool> IsReferencedInAnyCollectionAsync(string id, string entityType)
        {
            ObjectId objectId;

            if (!ObjectId.TryParse(id, out objectId))
            {
                throw new ArgumentException("ID không hợp lệ");
            }

            if (!ReferenceMapping.ContainsKey(entityType))
            {
                throw new ArgumentException($"Không tìm thấy cấu hình cho loại đối tượng {entityType}");
            }

            var tasks = new List<Task<long>>();

            foreach (var collectionName in ReferenceMapping.Keys)
            {
                var referenceFields = ReferenceMapping[collectionName];
                var collection = _mongoContext.GetCollection<BsonDocument>(collectionName);

                foreach (var referenceField in referenceFields)
                {
                    if (referenceField == "Id" || ReferenceMapping[entityType].Contains(referenceField))
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq(referenceField, objectId);
                        tasks.Add(collection.CountDocumentsAsync(filter));
                        Console.WriteLine($"Checking collection: {collectionName}, field: {referenceField}, id: {id}");
                    }
                }
            }

            var results = await Task.WhenAll(tasks);
            bool isReferenced = results.Any(count => count > 0);
            Console.WriteLine($"IsReferenced: {isReferenced}");
            return isReferenced;
        }

    }
}
