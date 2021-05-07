using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class Shopcollection
    {

        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int shopId { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public string shopName { get; set; }
        [BsonElement]
        public IList<int> shopItems = new List<int>();
    }
    
}
