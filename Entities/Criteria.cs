using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Criteria : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("points"), BsonRepresentation(BsonType.Int32)]
        public int Points { get; set; }

        [BsonElement("notes"), BsonRepresentation(BsonType.String)]
        public string? Notes { get; set; }

        [BsonElement("personCheck"), BsonRepresentation(BsonType.String)]
        public string? PersonCheck { get; set; }  

        [BsonElement("criteriaGroupId"), BsonRepresentation(BsonType.ObjectId)]
        public string? CriteriaGroupId { get; set; }

        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }
    }
}
