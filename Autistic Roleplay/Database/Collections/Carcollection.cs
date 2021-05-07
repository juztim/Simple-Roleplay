
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class Carcollection
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
        public int ownerId { get; set; }
        [BsonElement]
        public bool parkedIn { get; set; }
        [BsonElement]
        public double fuel { get; set; }
        [BsonElement]
        public int fuelType { get; set; }
        [BsonElement]
        public double kilometer { get; set; }
        [BsonElement]
        public List<int> allowedIds { get; set; } 
    }

    public class AppCar
    {
        public string garageId { get; set; }
        public string numPlate { get; set; }
        public string ownerId  { get; set; }
        public string carmodel { get; set; }
        public Pos pos { get; set; }
        public bool parkedIn { get; set; }

    }
    
}
