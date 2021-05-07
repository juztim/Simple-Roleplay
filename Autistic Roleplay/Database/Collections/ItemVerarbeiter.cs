using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class ItemVerarbeiter
    {
        [BsonElement]
        public int VerarbeiterId { get; set; }
        [BsonElement]
        public int CurrentIn { get; set; }
        [BsonElement]
        public int CurrentOut { get; set; }
    }
}
