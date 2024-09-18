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

        [BsonElement("points"), BsonRepresentation(BsonType.Double)]
        public double Points { get; set; }

        [BsonElement("role"), BsonRepresentation(BsonType.String)]
        public string? Role { get; set; }  // Addition or Subtraction

        [BsonElement("notes"), BsonRepresentation(BsonType.String)]
        public string? Notes { get; set; }

        [BsonElement("personCheck"), BsonRepresentation(BsonType.ObjectId)]
        public string? PersonCheck { get; set; }  // PositionId
    }
}
