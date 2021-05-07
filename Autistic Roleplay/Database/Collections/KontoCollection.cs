using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace Simple_Roleplay.Database.Collections
{
    public class KontoCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public ObjectId objectOwnerId { get; set; }
        [BsonElement]
        public int kontoStand { get; set; }
        [BsonElement]
        public int kontoNummer { get; set; }
        [BsonElement]
        public int kontoPin { get; set; }
       
    }
}
