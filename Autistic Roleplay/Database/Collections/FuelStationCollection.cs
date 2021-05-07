using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class FuelStationCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public Pos pos { get; set; }
        [BsonElement]
        public int id { get; set; }
    }
}
