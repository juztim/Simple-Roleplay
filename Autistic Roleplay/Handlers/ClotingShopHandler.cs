using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Handlers
{
    public class ClotingShopHandler : IScript
    {
        [ClientEvent("server:clothing:buy")]
        public async Task BuyClothing(User player, string clothId)
        {
            var dbClothing = Main.database.ClothesCollection.AsQueryable().FirstOrDefault(c => c.clothId == clothId);
            var price = 0;

            switch (int.Parse(dbClothing.shopType))
            {
                //Ponsonboys
                case 1:
                    price = 30000;
                    break;
                //Suburban
                case 2:
                    price = 5000;
                    break;
                //Discount
                case 3:
                    price = 2000;
                    break;
            }

            if (!await player.removeMoneyAsync(price)) return;
            {
                var dbchar = Utils.Utils.GetDatabaseCharacter(player);
                if (dbchar.OwnedClothes.Contains(int.Parse(dbClothing.clothId)))
                {
                    await NotifyHandler.SendNotification(player, "Du besitzt dies bereits!", 3000);
                    return;
                }
                dbchar.OwnedClothes.Add(int.Parse(dbClothing.clothId));
                switch (int.Parse(dbClothing.componentId))
                {
                    case 1:
                        //Maske
                        break;
                    case 2:
                        //Hair
                        break;
                    case 3:
                        //Torsos
                        break;
                    case 4:
                        //Legs
                        dbchar.Clothes.Legs = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                        break;
                    case 5:
                        //Bags
                        break;
                    case 6:
                        //Shoes
                        dbchar.Clothes.Shoes = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                        break;
                    case 7:
                        //Accessories
                        break;
                    case 8:
                        //Undershirts
                        dbchar.Clothes.Undershirt = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                        break;
                    case 9:
                        //Body Armors
                        break;
                    case 10:
                        //Decals
                        break;
                    case 11:
                        //Tops
                        dbchar.Clothes.Tops = new Component() {colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId)};
                        break;
                }

                await NotifyHandler.SendNotification(player, "Erfolgreich gekauft!", 5000);
                Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbchar);


            }

            return;
        }

        [ClientEvent("server:clothing:reset")]
        public async Task ResetClothing(User player)
        {
            await Utils.Utils.LoadClothes(player, Utils.Utils.GetDatabaseCharacter(player));
        }

        [ClientEvent("server:closet:equipClothing")]
        public async Task EquipClothing(User player, string clothId)
        {
            Console.WriteLine("EQUIP: " + clothId);
            var dbchar = Utils.Utils.GetDatabaseCharacter(player);
            var dbClothing = Main.database.ClothesCollection.AsQueryable().FirstOrDefault(c => clothId == c.clothId);

            switch (int.Parse(dbClothing.componentId))
            {
                    
                case 1:
                    //Maske
                    dbchar.Clothes.Mask = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 2:
                    //Hair
                    break;
                case 3:
                    //Torsos
                    dbchar.Clothes.Torso = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 4:
                    //Legs
                    dbchar.Clothes.Legs = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 5:
                    //Bags
                    break;
                case 6:
                    //Shoes
                    dbchar.Clothes.Shoes = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 7:
                    //Accessories
                    dbchar.Clothes.Accessoires = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 8:
                    //Undershirts
                    dbchar.Clothes.Undershirt = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
                case 9:
                    //Body Armors
                    dbchar.Clothes.Armor = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    if (int.Parse(dbClothing.clothId) != 15030) player.Armor = 100;
                    if (int.Parse(dbClothing.clothId) == 15030) player.Armor = 0;
                    break;
                case 10:
                    //Decals
                    break;
                case 11:
                    //Tops
                    dbchar.Clothes.Tops = new Component() { colorId = int.Parse(dbClothing.colorId), drawableId = int.Parse(dbClothing.drawableId) };
                    break;
            }

            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbchar);
        }
    }
}
