using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class GarageCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int garageId { get; set; }
        [BsonElement]
        public string Name { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public IList<Ausparkpunkt> Ausparkpunkte { get; set; }

    }
}
