using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;

namespace Simple_Roleplay.Handlers
{
    public class ItemShopHandler : IScript
    {
        [ClientEvent("client:itemshop:buyItem")]
        public async Task buyShopItems(User player, string str_itemId, string str_amount)
        {
            int itemId = int.Parse(str_itemId);
            int amount = int.Parse(str_amount);
            var price = Main.database.ItemCollection.AsQueryable().ToList().FirstOrDefault(i => i.itemId == itemId).price;

            if(price != null && await player.removeMoneyAsync(price.Value * amount))
            {
                player.addItem(itemId, amount);
                await NotifyHandler.SendNotification(player, $"{amount}x gekauft!", 1000);
            }
        }
    }
}
