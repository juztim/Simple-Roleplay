using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{

    public class ClientConversation
    {
        public string ChatName { get; set; }
        public BsonDateTime LastMessageDate { get; set; }
        public string LastMessageContent { get; set; }
        public List<ClientMessage> Messages { get; set; } = new List<ClientMessage>();
        public Guid Id { get; set; }
        public bool HasUnreadMessages { get; set; }
    }

    public class ClientMessage
    {
        public string Content { get; set; }
        public BsonDateTime Date {get; set; }
        public bool selfMessage { get; set; }
        public int Sender { get; set; }
        public Guid MessageId { get; set; }
        
    }

    public class ChatsRead
    {
        public Guid ChatId { get; set; }
        public BsonDateTime LastOpened { get; set; }
    }
    
    public class ChatHandler : IScript
    {
        [ClientEvent("server:chats:requestPreviews")]
        public Task requestChatPreviews(User player)
        {
            //Find Database Char
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            //Find Conversations
            var conversationsRaw = Main.database.ChatCollection.AsQueryable()
                .Where(c => c.Participants.Contains(dbChar.Number));
            
            var clientConversations = new List<ClientConversation>();
            //Find Name of Contact
            foreach (var conversation in conversationsRaw)
            {
                string ChatName;
                if (conversation.Participants.Count > 2)
                {
                    ChatName = conversation.GroupChatName;
                }
                else
                {
                    var dbContact = dbChar.Kontakte.FirstOrDefault(c =>
                        c.phoneNumber != dbChar.Number && conversation.Participants.Contains(c.phoneNumber));
                    ChatName = dbContact != null ? dbContact.Name : conversation.Participants.First(number => number != dbChar.Number).ToString();
                }

                var msgList = (from msg in conversation.Messages let isSelfMessage = msg.Sender == dbChar.Number select new ClientMessage {Content = msg.Content, Date = msg.Date, Sender = msg.Sender, selfMessage = isSelfMessage, MessageId = msg.Guid}).ToList();
                clientConversations.Add(new ClientConversation
                {
                    ChatName = ChatName, LastMessageContent = conversation.LastMessageContent,
                    LastMessageDate = conversation.LastMessageDate, Messages = msgList,
                    Id = conversation.Guid
                });
            }
            //Emit to Client
            player.Emit("client:chat:loadChats", JsonConvert.SerializeObject(clientConversations));

            return Task.CompletedTask;
        }

        [ClientEvent("server:chats:openChat")]
        public Task openChat(User player, string uid)
        {
            Console.WriteLine("Opening Chat: " + uid);
            //Find Database Char
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            //Find Database Conversation
            var dbConversation = Main.database.ChatCollection.AsQueryable().FirstOrDefault(c => new Guid(uid) == c.Guid);
            //Fetch all Messages
            var messagesRaw = dbConversation.Messages;
            var messageList = messagesRaw.Select(msg => new ClientMessage
                {
                    Content = msg.Content,
                    Date = msg.Date,
                    selfMessage = msg.Sender == dbChar.Number,
                    Sender = msg.Sender,
                    MessageId = msg.Guid
                })
                .ToList();
            var chat = new ClientConversation{Id = dbConversation.Guid, Messages = messageList, ChatName = dbConversation.GroupChatName, LastMessageContent = dbConversation.LastMessageContent, LastMessageDate = dbConversation.LastMessageDate};

            Console.WriteLine("Found " + messageList.Count + " Messages!");
            player.Emit("client:chat:fillChat", JsonConvert.SerializeObject(chat));


            return Task.CompletedTask;
        }

        [ClientEvent("server:chats:sendMessage")]
        public Task sendMessage(User player, string messageContent, string chat)
        {
            //Find Database Char
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            //Find Database Conversation
            var dbConvo = Main.database.ChatCollection.AsQueryable().FirstOrDefault(c => new Guid(chat) == c.Guid);
            dbConvo.Messages.Add(new Message
            {
                Content = messageContent,
                Date = new BsonDateTime(DateTime.Now),
                Guid = Guid.NewGuid(),
                Sender = dbChar.Number
            });
            Main.database.ChatCollection.ReplaceOne(c => new Guid(chat) == c.Guid, dbConvo);

            foreach (var receiver in dbConvo.Participants)
            {
                var dbReceiver = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.Number == receiver);
                if(dbReceiver == null) continue;
                if(!dbReceiver.isOnline) continue;
                var clientMsg = new ClientMessage
                {
                    Content = messageContent,
                    Date = new BsonDateTime(DateTime.Now),
                    Sender = dbChar.Number,
                    selfMessage = dbChar.Number == dbReceiver.Number,
                    MessageId = Guid.NewGuid()
                };
                player.Emit("client:chat:incomingmessage", dbConvo.Guid.ToString(), JsonConvert.SerializeObject(clientMsg));
            }

            return Task.CompletedTask;
        }
        
    }
}