using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Rank : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("pointRangeStart"), BsonRepresentation(BsonType.Int32)]
        public int? PointRangeStart { get; set; }
        [BsonElement("pointRangeEnd"), BsonRepresentation(BsonType.Int32)]
        public int? PointRangeEnd { get; set; }
        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }
    }
}
