using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class frakCarCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int carId { get; set; }
        [BsonElement]
        public string carmodel { get; set; }
        [BsonElement]
        public string numPlate { get; set; }
        [BsonElement]
        public int garageId { get; set; }
        [BsonElement]
        public bool parkedIn { get; set; }
        [BsonElement]
        public double fuel { get; set; }
        [BsonElement]
        public int fuelType { get; set; }
        [BsonElement]
        public double kilometer { get; set; }
        [BsonElement]
        public int frakId { get; set; }
    }
}
