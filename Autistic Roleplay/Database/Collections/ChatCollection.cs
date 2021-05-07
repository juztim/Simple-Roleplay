using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Database.Collections
{
    public class ChatCollection
    {
        [BsonId]
        public ObjectId objectId { get; set; }
        [BsonElement]
        public IList<int> Participants { get; set; } = new List<int>();
        [BsonElement]
        public string? GroupChatName { get; set; }
        [BsonElement]
        public BsonDateTime LastMessageDate { get; set; }
        [BsonElement]
        public string LastMessageContent { get; set; }
        [BsonElement]
        public IList<Message> Messages { get; set; } = new List<Message>();
        [BsonElement]
        public Guid Guid { get; set; }
        
    }

    public class Message
    {
        [BsonElement]
        public string Content { get; set; }
        [BsonElement]
        public BsonDateTime Date {get; set; }
        [BsonElement]
        public int Sender { get; set; }
        [BsonElement]
        public Guid Guid { get; set; }
    }
}