using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class ClothingStoreCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int storeType { get; set; }
        [BsonElement]
        public int storeId { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public int? propId { get; set; }
    }
}
