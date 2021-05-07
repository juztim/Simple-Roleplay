using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Simple_Roleplay.Handlers
{
    class FrakShopHandler : IScript
    {
        [ClientEvent("server:frakshop:buyItem")]
        public static async Task buyFrakItemAsync(User player, string str_itemid, string str_amount)
        {
            int itemId = Int32.Parse(str_itemid);
            int amount = Int32.Parse(str_amount);
            int frakid = player.frakId;

            var dbFrak = Main.database.FractionCollection.AsQueryable().FirstOrDefault(f => f.frakId == frakid);
            var dbFrakItems = Main.database.FrakNpcCollection.AsQueryable().FirstOrDefault(n => n.frakId == frakid).frakShopItems;
            var dbCharRank = dbFrak.frakMember.FirstOrDefault(c => c.playerid == player.playerId).rank;
            var dbFrakItem = dbFrakItems.FirstOrDefault(f => f.itemId == itemId);

            if(dbFrakItem.rank > dbCharRank)
            {
                await NotifyHandler.SendNotification(player, $"Dein Rang ist nicht ausreichend! ({dbFrakItem.rank})");
                return ;
            }
            if (await player.removeMoneyAsync(dbFrakItem.itemPrice * amount))
            {
                player.addItem(itemId, amount);
                await NotifyHandler.SendNotification(player, $"{amount}x gekauft!", 1000);
            }


        }
    }
}
