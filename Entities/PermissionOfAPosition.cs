using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class PermissionOfAPosition : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("positionId"), BsonRepresentation(BsonType.ObjectId)]
        public string? PositionId { get; set; }

        [BsonElement("permissionId"), BsonRepresentation(BsonType.ObjectId)]
        public string? PermissionId { get; set; }
    }
}
