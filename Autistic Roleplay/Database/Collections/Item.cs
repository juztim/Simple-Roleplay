using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class Item
    {
        [BsonElement]
        public int itemId { get; set; }
        [BsonElement]
        public int amount { get; set; }
    }
}
