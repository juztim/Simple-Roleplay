using AltV.Net;
using AltV.Net.Elements.Entities;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AltV.Net.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Handlers;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay
{
    public class Main : Resource
    {
        public static Database.Database database = new Database.Database();
        public override IEntityFactory<IPlayer> GetPlayerFactory()
        {
            return new UserFactory();
        }

        public override IEntityFactory<IVehicle> GetVehicleFactory()
        {
            return new VehicleFactory();
        }
        
        public override async void OnStart()
        {
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("[SimpleRoleplay] Started!");
            Discord.Webhookhandler.sendMessage("----------START----------");
            database.InitDB();
            await interactionHandler.LoadInteractions();
            await interactionHandler.LoadVehicleShops();
            await Utils.Utils.Clothes.LoadClothesDB();
            await NametagHandlers.Init();
            await ItemProducerHandler.Init();
            await JailHandler.initJailTimer();

            //var bot = new Discord.BotMain();
            //bot.RunAsync().GetAwaiter().GetResult();

            //1 = Ponsonboys 2 = Suburban 3 = Discount, 4 = Badestore, 5 = Maskenladen, 6 = Brillen, 7 = Hutladen, 8 = Juwe, 9 = Alle
            var emp = Alt.CreateColShapeSphere(
                new Position(1692.7078857421875f, 2599.947021484375f, 45.56489944458008f), 200f);

            /*var newConvo = new ChatCollection
            {
                Participants = new List<int> {1, 2}, LastMessageContent = "Last Message",
                LastMessageDate = new BsonDateTime(DateTime.Now), Guid = Guid.NewGuid(),
                Messages = new List<Message>
                {
                    new Message
                    {
                        Content = "Test Message", Date = new BsonDateTime(DateTime.Now), Sender = 65944,
                        Guid = Guid.NewGuid()
                    }
                }
            };
            await database.chatCollection.InsertOneAsync(newConvo);*/
            
        }

        public override void OnStop()
        {
            Utils.Utils.SaveAllPlayerPos();
            Alt.Log("[SimpleRoleplay] Saved all positions");
            Discord.Webhookhandler.sendMessage("----------STOP----------");
            Utils.Utils.ParkInVehicles();

        }

    }
}
