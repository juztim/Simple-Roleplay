using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AltV.Net.Enums;
using AltV.Net.Elements.Entities;

namespace Simple_Roleplay.Handlers
{
    public class InventoryHandler : IScript
    {
        [ClientEvent("server:requestInv")]
        public Task requestInventory(User player)
        {
            var itemList = new List<ClientItem>();
            
            var playerInventory = Utils.Utils.GetDatabaseCharacter(player).Inventar;
            foreach (var item in playerInventory)
            {
                if (item.amount > 0)
                {
                    var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == item.itemId);
                    var newItem = new ClientItem() { amount = item.amount, itemName = dbItem.name, weight = dbItem.weight, itemId = dbItem.itemId };
                    itemList.Add(newItem);
                }
            }
            player.Emit("client:hud:loadInventory", JsonConvert.SerializeObject(itemList));
            return Task.CompletedTask;
        }

        [ClientEvent("server:useItem")]
        public async Task useItem(User player, string str_itemId, string str_amount)
        {
            var dbitem = Main.database.ItemCollection.AsQueryable()
                .FirstOrDefault(i => i.itemId == int.Parse(str_itemId));
            if(dbitem == null) return;
            if(!dbitem.isUseable) return;
            

            switch (int.Parse(str_itemId))
            {
                //Verbandkasten
                case 29:
                    if (!await player.removeItem(int.Parse(str_itemId), int.Parse(str_amount)))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    player.Emit("client:playAnimation", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 4500, 1);
                    await Task.Delay(4500);
                    player.Health = 200;
                    break;
                //Pistole
                case 9:
                    if (!await player.removeItem(int.Parse(str_itemId), 1))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    player.GiveWeapon(WeaponModel.Pistol, 0, true);
                    break;
                //Deagle
                case 10:
                    if (!await player.removeItem(int.Parse(str_itemId), 1))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    player.GiveWeapon(WeaponModel.Pistol50, 0, true);
                    break;
                //Pistol ammo
                case 11:
                    if (!await player.removeItem(int.Parse(str_itemId), int.Parse(str_amount)))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    if (player.CurrentWeapon != (uint) WeaponModel.Pistol)
                    {
                        NotifyHandler.SendNotification(player, "Du hast keine Pistole in der Hand!");
                        return;
                    }
                    player.GiveWeapon(WeaponModel.Pistol, int.Parse(str_amount), true);
                    break;
                //Deagle Ammo
                case 12:
                    if (!await player.removeItem(int.Parse(str_itemId), int.Parse(str_amount)))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    if (player.CurrentWeapon != (uint) WeaponModel.Pistol50)
                    {
                        NotifyHandler.SendNotification(player, "Du hast keine Deagle in der Hand!");
                        return;
                    }
                    player.GiveWeapon(WeaponModel.Pistol50, int.Parse(str_amount),true);
                    break;
                //Taser
                case 18:
                    if (!await player.removeItem(int.Parse(str_itemId), 1))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    if (player.frakId != 2 && player.frakId != 1) return;
                    player.GiveWeapon(WeaponModel.StunGun, 0, true);
                    break;
                //Weste
                case 34:
                    if (!await player.removeItem(int.Parse(str_itemId), 1))
                    {
                        await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
                        return;
                    }
                    player.Emit("client:playAnimation", "anim@heists@narcotics@funding@gang_idle", "gang_chatting_idle01", 4500, 1);
                    await Task.Delay(4500);
                    player.Armor = 100;
                    break;




            }
            return;
        }

        [ClientEvent("server:removeItem")]
        public async Task removeItem(User player, string str_id, string str_amount)
        {
            
            if(!await player.removeItem(int.Parse(str_id), int.Parse(str_amount)))
            {
                await NotifyHandler.SendNotification(player, "Fehler beim Entfernen!");
            }
        }

    }
}
