
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using Simple_Roleplay.Handlers;

namespace Simple_Roleplay.Database.Collections
{

    public class Contact
    {
     [BsonElement]
     public string Name { get; set; }
     [BsonElement]
     public int phoneNumber { get; set; }
    }
    
    public class Characters
    {
        [BsonId]
        public ObjectId id { get; set; }
        [BsonElement]
        public int playerId { get; set; }
        [BsonElement]
        public string firstName { get; set; }
        [BsonElement]
        public string lastName { get; set; }
        [BsonElement]
        public string birthDay { get; set; }
        [BsonElement]
        public string birthplace { get; set; }
        [BsonElement]
        public ushort health { get; set; }
        [BsonElement]
        public ushort armor { get; set; }
        [BsonElement]
        public float? pos_x { get; set; }
        [BsonElement]
        public float? pos_y { get; set; }
        [BsonElement]
        public float? pos_z { get; set; }
        [BsonElement]
        public Chardata charData { get; set; }
        [BsonElement]
        public int moneyHand { get; set; }
        [BsonElement]
        public Clothing Clothes { get; set; }
        [BsonElement]
        public IList<Item> Inventar { get; set; }
        [BsonElement]
        public ObjectId ownerObjId { get; set; }
        [BsonElement]
        public int? frakId { get; set; }
        [BsonElement]
        public bool isOnline { get; set; }
        [BsonElement]
        public IList<dynamic> weapons { get; set; } = new List<dynamic>();
        [BsonElement]
        public int Number { get; set; }
        [BsonElement]
        public IList<int> OwnedClothes { get; set; } = new List<int>();
        [BsonElement]
        public ItemVerarbeiter ItemVerarbeiter { get; set; } = new ItemVerarbeiter();
        [BsonElement]
        public IList<int> Akten { get; set; } = new List<int>();
        [BsonElement]
        public int jailTime { get; set; }
        [BsonElement]
        public IList<Contact> Kontakte { get; set; } = new List<Contact>();
        [BsonElement]
        public IList<ChatsRead> ChatsReads { get; set; } = new List<ChatsRead>(); 
    }
}
