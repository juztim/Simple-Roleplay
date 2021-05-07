using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class feldCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int id { get; set; }   
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public float radius { get; set; }
        [BsonElement]
        public Pos pos { get; set; }
        [BsonElement]
        public int itemId { get; set; }
        [BsonElement]
        public int itemMin { get; set; }
        [BsonElement]
        public int itemMax { get; set; }
    }
}
