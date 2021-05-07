using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Simple_Roleplay.Database.Collections
{
    public class Users
    {
        [JsonIgnore]
        [BsonId]
        public ObjectId id { get; set; }
        [BsonElement]
        public string Username { get; set; }
        [BsonElement]
        public string Password { get; set; }
        [BsonElement]
        public int adminLevel { get; set; }
        [BsonElement]
        public ulong hwid { get; set; }
        [BsonElement]
        public string lastLogin { get; set; }
        [BsonElement]
        public bool IsWhitelisted { get; set; }

    }
}
