using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIwithMongoDB.Entities
{
    public class Evaluate : IMongoEntity
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name"), BsonRepresentation(BsonType.String)]
        public string Name { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("rankId")]
        public string? RankId { get; set; }
        
        [BsonElement("totalPointSubstraction"), BsonRepresentation(BsonType.Int32)]
        public int? TotalPointSubstraction { get; set; }

        [BsonElement("totalPointAddition"), BsonRepresentation(BsonType.Int32)]
        public int? TotalPointAddition { get; set; }

        [BsonElement("stt"), BsonRepresentation(BsonType.Int32)]
        public int Stt { get; set; }

        [BsonElement("from"), BsonRepresentation(BsonType.DateTime)]
        public DateTime From { get; set; }

        [BsonElement("to"), BsonRepresentation(BsonType.DateTime)]
        public DateTime To { get; set; }

        [BsonElement("totalPoint"), BsonRepresentation(BsonType.Int32)]
        public int? TotalPoint { get; set; }

        [BsonElement("UploadDay"), BsonRepresentation(BsonType.DateTime)]
        public DateTime UploadDay { get; set; }

        [BsonElement("confirmDay"), BsonRepresentation(BsonType.DateTime)]
        public DateTime? ConfirmDay { get; set; }

        [BsonElement("TimeStamp"), BsonRepresentation(BsonType.DateTime)]
        public DateTime TimeStamp { get; set; }
    }
}
