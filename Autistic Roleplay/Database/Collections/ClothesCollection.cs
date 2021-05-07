using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace Simple_Roleplay.Database.Collections
{
    [DataContract]
    public class ClothesCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        [DataMember]
        public string clothId { get; set; }
        [BsonElement]
        public string gender { get; set; }
        [BsonElement]
        public string shopType { get; set; }
        [BsonElement]
        [DataMember]
        public string price { get; set; }
        [BsonElement]
        [DataMember]
        public string componentId { get; set; } 
        [BsonElement]
        [DataMember]
        public string drawableId { get; set; }
        [BsonElement]
        [DataMember]
        public string colorId { get; set; }
        [BsonElement]
        public string name { get; set; }
        [BsonElement]
        public string propId { get; set; }
    }

    public class ClientClothing
    {
        public int? ComponentId { get; set; }
        public int DrawableId { get; set; }
        public int ColorId { get; set; }
        public int ClothId { get; set; }
        public string? Name { get; set; }
        public int? PropId { get; set; }
    }
}
//1 = Ponsonboys 2 = Suburban 3 = Discount, 4 = Badestore, 5 = Maskenladen, 6 = Brillen, 7 = Hutladen, 8 = Juwe, 9 = Alle