using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class FrakGarageCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public int Id { get; set; }
        [BsonElement]
        public Pos Pos { get; set; }
        [BsonElement]
        public int FrakId { get; set; }
        [BsonElement] 
        public IList<Ausparkpunkt> Ausparkpunkte { get; set; } = new List<Ausparkpunkt>();

    }
}
