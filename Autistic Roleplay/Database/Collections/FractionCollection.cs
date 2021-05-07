using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class FractionCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public int frakId { get; set; }
        [BsonElement]
        public string frakName { get; set; }
        [BsonElement]
        public bool isBadFrak { get; set; }
        [BsonElement]
        public List<FrakMember> frakMember { get; set; } = new List<FrakMember>();
        

    }

    public class AppFrak
    {
        public string frakName { get; set; }
        public List<AppMember> frakMember { get; set; } = new List<AppMember>();
    }

    public class AppMember
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int rank { get; set; }
    }
}
