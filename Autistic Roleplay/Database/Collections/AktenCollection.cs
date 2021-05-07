using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Roleplay.Database.Collections
{
    public class AktenCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public string CategoryName { get; set; }
        [BsonElement]
        public int CategoryId { get; set; }
        [BsonElement]
        public IList<Akte> Akten { get; set; } = new List<Akte>();
        
    }

    public class Akte
    {
        public int AktenId { get; set; }
        public string Name { get; set; }
        public int Fine { get; set; }
        public int JailTime { get; set; }
    }

}
