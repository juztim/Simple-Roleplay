using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simple_Roleplay.Handlers
{
    public class FeldHandler : IScript
    {
        [ClientEvent("server:feld:farmen")]
        public async Task Farmhandler(User player, long feldId)
        {
           

            var dbFeld = Main.database.FeldCollection.AsQueryable().FirstOrDefault(f => f.id == feldId);
            var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == dbFeld.itemId);
            var random = new Random();
            var itemAmount = random.Next(dbFeld.itemMin, dbFeld.itemMax + 1);

            await NotifyHandler.SendNotification(player, $"Du hast {dbItem.name} ({itemAmount}x) gesammelt!");
            player.addItem(dbFeld.itemId, itemAmount);
        }
    }
}
