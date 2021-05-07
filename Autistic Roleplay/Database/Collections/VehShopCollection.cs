using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class VehShopCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public int Id { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public IList<VehShopCar> Cars { get; set; } = new List<VehShopCar>();
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public bool Visible { get; set; }
        [BsonElement]
        public Pos SpawnPosition { get; set; }
        [BsonElement]
        public Rot SpawnRotation { get; set; }
    }

    public class VehShopCar
    {
        public string Model { get; set; }
        public Pos Pos { get; set; }
        public int Price { get; set; }
        public Rot Rotation { get; set; }

    }
}
