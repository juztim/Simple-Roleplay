using MongoDB.Driver;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Simple_Roleplay.Utils
{
    class Utils
    {
        public static IList<ClothesCollection> CachedColthing { get; set; }
        public static Users GetDatabasePlayer(User player)
        {
            return Main.database.UserCollection.AsQueryable().FirstOrDefault(p => p.Username == player.Username);
        }
        public static Characters GetDatabaseCharacter(Users dbUser)
        {
            return Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == dbUser.id);
        }
        public static Characters GetDatabaseCharacter(User player)
        {
            return Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
        }

        public static Characters GetDatabaseCharacter(int playerId)
        {
            foreach(User player in Alt.GetAllPlayers())
            {
                if(player.playerId == playerId)
                {
                    return GetDatabaseCharacter(player);
                }      
            }
            return null;
        }

        public static float ToRadiants(float val)
        {
            return (float)(val * (Math.PI / 180));
        }

        public static float ToDegrees(float val)
        {
            return (float)(val * (180 / Math.PI));
        }

        internal static void SaveAllPlayerPos()
        {
            var playerPool = Alt.GetAllPlayers();
            foreach (var player in playerPool)
            {
                var tempPlayer = player as User;

                var dbUser = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(u => u.ownerObjId == tempPlayer.uid);
                var tempDbUser = dbUser;

                tempDbUser.pos_x = tempPlayer.Position.X;
                tempDbUser.pos_y = tempPlayer.Position.Y;
                tempDbUser.pos_z = tempPlayer.Position.Z;

                Main.database.CharacterCollection.ReplaceOne(u => u.ownerObjId == tempPlayer.uid, tempDbUser);
            }
        }

        internal static async Task LoadClothes(User player, Characters dbChar)
        {



            var shoes = new
            {
                componentId = 6,
                dbChar.Clothes.Shoes.drawableId,
                dbChar.Clothes.Shoes.colorId
            };

            var legs = new
            {
                componentId = 4,
                dbChar.Clothes.Legs.drawableId,
                dbChar.Clothes.Legs.colorId
            };

            var tops = new
            {
                componentId = 11,
                dbChar.Clothes.Tops.drawableId,
                dbChar.Clothes.Tops.colorId
            };


            var undershirts = new
            {
                componentId = 8,
                dbChar.Clothes.Undershirt.drawableId,
                dbChar.Clothes.Undershirt.colorId
            };

            var accessoires = new
            {
                componentId = 7,
                dbChar.Clothes.Accessoires.drawableId,
                dbChar.Clothes.Accessoires.colorId
            };

            var armor = new
            {
                componentId = 9,
                dbChar.Clothes.Armor.drawableId,
                dbChar.Clothes.Armor.colorId
            };

            var bags = new
            {
                componentId = 5,
                dbChar.Clothes.Armor.drawableId,
                dbChar.Clothes.Armor.colorId
            };

            var decals = new
            {
                componentId = 10,
                dbChar.Clothes.Decals.drawableId,
                dbChar.Clothes.Decals.colorId
            };

            var masks = new
            {
                componentId = 1,
                dbChar.Clothes.Mask.drawableId,
                dbChar.Clothes.Mask.colorId
            };
            var torso = new
            {
                componentId = 3,
                dbChar.Clothes.Torso.drawableId,
                dbChar.Clothes.Torso.colorId
            };

            var clothes = new[] { shoes, legs, tops, undershirts, accessoires, armor, decals, masks, torso}.ToList();
            player.Emit("client:clothes:loadClothes", JsonConvert.SerializeObject(clothes));
        }

        public class Clothes
        {
            public static List<ClothesCollection> maskList { get; set; }
            public static List<ClothesCollection> torsoList { get; set; }
            public static List<ClothesCollection> legList { get; set; }
            public static List<ClothesCollection> shoesList { get; set; }
            public static List<ClothesCollection> topsList { get; set; }
            public static List<ClothesCollection> undershirtsList { get; set; }
            public static List<ClothesCollection> armorList { get; set; }



            public static List<ClothesCollection> discountList { get; set; }
            public static List<ClothesCollection> suburbanList { get; set; }
            public static List<ClothesCollection> ponsonList { get; set; }

            internal static async Task LoadClothesDB()
            {
                maskList = new List<ClothesCollection>();
                torsoList = new List<ClothesCollection>();
                legList = new List<ClothesCollection>();
                shoesList = new List<ClothesCollection>();
                topsList = new List<ClothesCollection>();
                undershirtsList = new List<ClothesCollection>();
                armorList = new List<ClothesCollection>();
                discountList = new List<ClothesCollection>();
                suburbanList = new List<ClothesCollection>();
                ponsonList = new List<ClothesCollection>();



                var dbClothesList = Main.database.ClothesCollection.AsQueryable().ToList();

                foreach (var cloth in dbClothesList)
                {
                    if (cloth.componentId.Trim() == "") continue;
                    switch (int.Parse(cloth.componentId))
                    {
                        case 1:
                            maskList.Add(cloth);
                            break;
                        case 2:
                            break;
                        case 3:
                            torsoList.Add(cloth);
                            break;
                        case 4:
                            legList.Add(cloth);
                            break;
                        case 5:
                            break;
                        case 6:
                            shoesList.Add(cloth);
                            break;
                        case 7:
                            break;
                        case 8:
                            undershirtsList.Add(cloth);
                            break;
                        case 9:
                            armorList.Add(cloth);
                            break;
                        case 10:
                            break;
                        case 11:
                            topsList.Add(cloth);
                            break;
                    }

                    switch (int.Parse(cloth.shopType))
                    {
                        //PB
                        case 1:
                            ponsonList.Add(cloth);
                            break;
                        //Suburban
                        case 2:
                            suburbanList.Add(cloth);
                            break;
                        //Discount
                        case 3:
                            discountList.Add(cloth);
                            break;
                        //All
                        case 9:
                            ponsonList.Add(cloth);
                            suburbanList.Add(cloth);
                            discountList.Add(cloth);
                            break;
                    }

                }

               


                

                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("[SimpleRoleplay] " + maskList.Count + " Masken geladen!");
                Console.WriteLine("[SimpleRoleplay] " + torsoList.Count + " Torsos geladen!");
                Console.WriteLine("[SimpleRoleplay] " + legList.Count + " Hosen geladen!");
                Console.WriteLine("[SimpleRoleplay] " + shoesList.Count + " Schuhe geladen!");
                Console.WriteLine("[SimpleRoleplay] " + topsList.Count + " Tops geladen!");
                Console.WriteLine("[SimpleRoleplay] " + undershirtsList.Count + " Undershirts geladen!");
                Console.WriteLine("[SimpleRoleplay] " + armorList.Count + " Armors geladen!");
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine("[SimpleRoleplay] " + discountList.Count + " Discountstore Artikel geladen!");
                Console.WriteLine("[SimpleRoleplay] " + suburbanList.Count + " Suburban Artikel geladen!");
                Console.WriteLine("[SimpleRoleplay] " + ponsonList.Count + " Ponsonboys Artikel geladen!");
                Console.WriteLine("------------------------------------------------");
                return;
            }
        }


        internal static Task LoadPlayerWeapons(User player)
        {
            var dbChar = GetDatabaseCharacter(player);
            foreach (var weapon in dbChar.weapons)
            {
                player.GiveWeapon(weapon, 0, false);
                dbChar.weapons.Remove(weapon);
            }
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == player.uid, dbChar);
            return Task.CompletedTask;
        }
        internal static User FindPlayer(int wantedId)
        {
            
            
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (User p in Alt.GetAllPlayers())
            {
                if(p.playerId == wantedId)
                {
                    return p;
                }
            }

            return null;
        }

        public static async Task ParkInVehicles()
        {
            var carPool = Alt.GetAllVehicles();
            foreach (Car veh in carPool)
            {
                if(veh.carId <= 0) continue;
                if (veh.frakId <= 0)
                {
                    var dbCar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.carId == veh.carId);
                    dbCar.parkedIn = true;
                    await Main.database.CarCollection.ReplaceOneAsync(c => c.carId == veh.carId, dbCar);
                }
                else if(veh.frakId > 0)
                {
                    var dbCar = Main.database.FrakcarCollection.AsQueryable().FirstOrDefault(c => c.carId == veh.carId);
                    dbCar.parkedIn = true;
                    await Main.database.FrakcarCollection.ReplaceOneAsync(c => c.carId == veh.carId, dbCar);
                }
                
            }
        }
    }
   
}
