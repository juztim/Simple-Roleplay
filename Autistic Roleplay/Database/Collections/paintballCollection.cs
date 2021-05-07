using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class PaintballCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int arenaId { get; set; }
        [BsonElement]
        public string arenaName { get; set; }
        [BsonElement]
        public int playerCount { get; set; }
        [BsonElement]
        public int playerMax { get; set; }
        [BsonElement]
        public int price { get; set; }
        [BsonElement]
        public List<Pos> spawnPoints { get; set; } = new List<Pos>();
    }
}
