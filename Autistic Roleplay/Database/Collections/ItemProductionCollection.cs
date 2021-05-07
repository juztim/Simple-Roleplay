using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class ItemProductionCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public int Id { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public bool IsVisible { get; set; }
        [BsonElement]
        public int NeededItemId { get; set; }
        [BsonElement]
        public int NeededItemAmount { get; set; }
        [BsonElement]
        public int OutComeItemId { get; set; }
        [BsonElement]
        public int OutComeAmount { get; set; }
    }
}
