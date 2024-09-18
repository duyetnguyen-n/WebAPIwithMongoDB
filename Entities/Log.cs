using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Log : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId"), BsonRepresentation(BsonType.ObjectId)]
        public string? UserId { get; set; }

        [BsonElement("action"), BsonRepresentation(BsonType.String)]
        public string? Action { get; set; }

        [BsonElement("timestamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }

        [BsonElement("status"), BsonRepresentation(BsonType.String)]
        public string? Status { get; set; }

        [BsonElement("description"), BsonRepresentation(BsonType.String)]
        public string? Description { get; set; }
    }
}