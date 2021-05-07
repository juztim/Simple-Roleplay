using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class Interactions
    {
        [BsonId]
        public ObjectId id { get; set; }

    }
}
