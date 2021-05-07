using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using Newtonsoft.Json;

namespace Simple_Roleplay.Handlers
{
    public class AmmunationHandler : IScript
    {
        [ClientEvent("server:ammunation:buy")]
        public async Task Ammunation_Buy(User player, string itemId, string amount, int ammunationId)
        {
            Console.WriteLine(ammunationId);
            var dbAmmunation = Main.database.AmmunationCollection.AsQueryable()
                .FirstOrDefault(a => a.Id == ammunationId);

            Console.WriteLine(JsonConvert.SerializeObject(dbAmmunation));
            var ammunationInv = dbAmmunation.Inventory;
            var dbItem = ammunationInv.FirstOrDefault(i => i.ItemId == int.Parse(itemId));
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);

            if (!await player.removeMoneyAsync(dbItem.Price * int.Parse(amount))) return;
            player.addItem(dbItem.ItemId, int.Parse(amount));
            await NotifyHandler.SendNotification(player, "Erfolgreich gekauft!", 3000);
        }
    }
}
