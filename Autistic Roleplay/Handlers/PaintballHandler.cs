using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System;
using System.Threading.Tasks;
using System.Linq;
using AltV.Net.Elements.Entities;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Handlers
{
    public class PaintballHandler : IScript
    {
        private static async Task SetPaintballClothes(User player)
        {
            player.Emit("client:setClothes", 1, 95, 0);
            await Task.Delay(10);
            player.Emit("client:setClothes", 3, 11, 0);
            await Task.Delay(10);
            player.Emit("client:setClothes", 4, 42, 0);
            await Task.Delay(10);
            player.Emit("client:setClothes", 6, 6, 0);
            await Task.Delay(10);
            player.Emit("client:setClothes", 11, 83, 0);
        }

        internal static void HandleDeath(User player)
        {
            var paintballArena = Main.database.PaintballCollection.AsQueryable().FirstOrDefault(p => p.arenaId == player.paintballArena);
            var paintballSpawns = paintballArena.spawnPoints.ToList();
            var rand = new Random();
            var randIndex = rand.Next(paintballSpawns.Count);
            player.Spawn(ATMS.atmInteraction.toAltPos(paintballSpawns.ElementAt(randIndex)), 0);
            player.RemoveAllWeapons();
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol50, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.HeavyPistol, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.AdvancedRifle, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.GusenbergSweeper, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPDW, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 9999, true);
           
            player.Health = 200;
            player.Armor = 100;

            player.Position = ATMS.atmInteraction.toAltPos(paintballSpawns.ElementAt(randIndex));
        }

        internal static void Spawn(User player)
        {
            player.RemoveAllWeapons();
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.Pistol50, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.HeavyPistol, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.AdvancedRifle, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.GusenbergSweeper, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPDW, 9999, true);
            player.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 9999, true);
            var paintballArena = Main.database.PaintballCollection.AsQueryable().FirstOrDefault(p => p.arenaId == player.paintballArena);
            var paintballSpawns = paintballArena.spawnPoints.ToList();
            var rand = new Random();
            var randIndex = rand.Next(paintballSpawns.Count);
            player.Health = 200;
            player.Armor = 100;

            player.Position = ATMS.atmInteraction.toAltPos(paintballSpawns.ElementAt(randIndex));

        }

        internal static void HandleDisconnect(User player)
        {
            var paintballArena = Main.database.PaintballCollection.AsQueryable().FirstOrDefault(p => p.arenaId == player.paintballArena);
            paintballArena.playerCount -= 1;
            Main.database.PaintballCollection.ReplaceOne(p => p.arenaId == player.paintballArena, paintballArena);
        }

        [ClientEvent("server:paintball:joinPaintball")]
        public static async Task Join(User player, string str_arena)
        {
            if (player.paintballArena == 0)
            {
                int arenaId = int.Parse(str_arena);
                var paintballArena = Main.database.PaintballCollection.AsQueryable().FirstOrDefault(p => p.arenaId == arenaId);
                if (paintballArena.playerCount < paintballArena.playerMax)
                {
                    paintballArena.playerCount += 1;

                    if (await player.removeMoneyAsync(paintballArena.price))
                    {
                        await NotifyHandler.SendNotification(player, "Du bist Paintball beigetreten!");
                        await player.SaveWeapons();
                        player.paintballArena = arenaId;      
                        player.Dimension = 1;
                        await SetPaintballClothes(player);
                        player.RemoveAllWeapons();
                        await Main.database.PaintballCollection.ReplaceOneAsync(p => p.arenaId == arenaId, paintballArena);
                        player.GiveWeapon(AltV.Net.Enums.WeaponModel.AdvancedRifle, 9999, true);
                        player.GiveWeapon(AltV.Net.Enums.WeaponModel.GusenbergSweeper, 9999, true);
                        player.GiveWeapon(AltV.Net.Enums.WeaponModel.CombatPDW, 9999, true);
                        player.GiveWeapon(AltV.Net.Enums.WeaponModel.StunGun, 9999, true);
                        Spawn(player);

                    }
                   
                }
                else
                {
                    await NotifyHandler.SendNotification(player, "Diese Arena ist voll!");
                }
                
            }
        }        
        

      
        
    }
}
