using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class AmmunitionCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public IList<Weapon> Inventory { get; set; } = new List<Weapon>();
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public int Id { get; set; }
    }

    public class Weapon
    {
        public int ItemId { get; set; }
        public int Price { get; set; }
    }
}
