using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class TeachGroup : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("count"), BsonRepresentation(BsonType.Int32)]
        public int Count { get; set; }
        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }
    }
}
