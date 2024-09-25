using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class User : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("numberPhone"), BsonRepresentation(BsonType.String)]
        public string? NumberPhone { get; set; }

        [BsonElement("password"), BsonRepresentation(BsonType.String)]
        public string? Password { get; set; }

        [BsonElement("position"), BsonRepresentation(BsonType.String)]
        public string? Position { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string? Name { get; set; }

        [BsonElement("age"), BsonRepresentation(BsonType.Int32)]
        public int Age { get; set; }

        [BsonElement("mail"), BsonRepresentation(BsonType.String)]
        public string? Mail { get; set; }

        [BsonElement("gender"), BsonRepresentation(BsonType.String)]
        public string? Gender { get; set; }

        [BsonElement("teachGroupId"), BsonRepresentation(BsonType.ObjectId)]
        public string? TeachGroupId { get; set; }

        [BsonElement("point"), BsonRepresentation(BsonType.Double)]
        public double Point { get; set; }

        [BsonElement("avatar"), BsonRepresentation(BsonType.String)]
        public string? Avatar { get; set; }
        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? TimeStamp { get; set; }

    }
}