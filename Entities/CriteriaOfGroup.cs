using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class CriteriaOfGroup : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("criteriaId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CriteriaId { get; set; }

        [BsonElement("criteriaGroupId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CriteriaGroupId { get; set; }
        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }
    }
}
