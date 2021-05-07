using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class VehicleModifierCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public string ModelName { get; set; }
        [BsonElement]
        public int VehSpeedModifier { get; set; }
    }
}
