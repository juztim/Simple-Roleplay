using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class ItemCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int itemId { get; set; }
        [BsonElement]
        public double weight { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public bool isUseable { get; set; }
        [BsonElement]
        public int? price { get; set; }
    }
}
