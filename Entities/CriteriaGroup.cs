using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class CriteriaGroup : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "Tên không được để trống")]
        public string? Name { get; set; }

        [BsonElement("count"), BsonRepresentation(BsonType.Int32)]
         public int Count { get; set; }

        [BsonElement("role"), BsonRepresentation(BsonType.String)]
        [Required(ErrorMessage = "Loại tiêu chí không được để trống")]
        public string? Role { get; set; }  
    }
}
