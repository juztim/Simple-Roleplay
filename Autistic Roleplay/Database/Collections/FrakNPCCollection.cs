using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class FrakNPCCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public Pos pos { get; set; }
        [BsonElement]
        public int frakId { get; set; }
        [BsonElement]
        public IList<FrakShopItem> frakShopItems = new List<FrakShopItem>();
    }
    public class FrakShopItem
    {
        public int itemId { get; set; }
        public int itemPrice { get; set; }
        public int itemMaxAmount { get; set; }
        public int rank { get; set; }
    }

    public class FrakNPCInteraction
    {
        public class FrakNPC
        {
            public Pos Pos { get; set; }
            public int frakId { get; set; }
            public IList<FrakShopItem> frakShopItems { get; set; } = new List<FrakShopItem>();
        }
        public static List<FrakNPC> frakNPCList { get; set; } = new List<FrakNPC>();
    }
}
