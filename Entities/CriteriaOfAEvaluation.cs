using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class CriteriaOfAEvaluation : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("evaluateId"), BsonRepresentation(BsonType.ObjectId)]
        public string? EvaluateId { get; set; }

        [BsonElement("criteriaId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CriteriaId { get; set; }

        [BsonElement("quantity"), BsonRepresentation(BsonType.Int32)]
        public int? Quantity { get; set; }

        [BsonElement("total"), BsonRepresentation(BsonType.Int32)]
        public int? Total { get; set; }
        
        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }
    }
}
