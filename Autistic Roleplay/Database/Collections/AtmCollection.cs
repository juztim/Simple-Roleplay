using MongoDB.Bson;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Database.Collections
{
    public class ATMCollection
    {
        public ObjectId _id { get; set; }
        public string Name { get; set; }
        public long Hash { get; set; }
        public Pos Position { get; set; }
        public Rot Rotation { get; set; }
    }
}
