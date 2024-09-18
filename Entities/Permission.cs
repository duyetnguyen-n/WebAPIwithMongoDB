using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Permission : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }
    }
}
