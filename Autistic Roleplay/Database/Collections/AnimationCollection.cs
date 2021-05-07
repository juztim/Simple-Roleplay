using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class AnimationCollection
    {
        [BsonId]
        public ObjectId ObjectId { get; set; }
        [BsonElement]
        public string AnimDict { get; set; }
        [BsonElement]
        public string AnimName { get; set; }
        [BsonElement]
        public int Duration { get; set; }
        [BsonElement]
        public int Flags { get; set; }
        [BsonElement]
        public string Name { get; set; }
        //1 = x, 2 = x, 3 = x
        [BsonElement]
        public int Category { get; set; }
        [BsonElement]
        public bool IsVisible { get; set; }
    }
}
