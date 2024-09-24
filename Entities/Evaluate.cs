using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Evaluate : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonElement("rankId"), BsonRepresentation(BsonType.ObjectId)]
        public string? RankId { get; set; }
        
        [BsonElement("totalPointSubstraction"), BsonRepresentation(BsonType.Double)]
        public double TotalPointSubstraction { get; set; }

        [BsonElement("totalPointAddition"), BsonRepresentation(BsonType.Double)]
        public double TotalPointAddition { get; set; }
    }
}
