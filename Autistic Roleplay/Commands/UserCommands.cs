using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using Simple_Roleplay.Factories;
using System.Threading.Tasks;
using Simple_Roleplay.Handlers;
using MongoDB.Driver;
using System.Linq;
using AltV.Net.Data;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Commands
{
    public class UserCommands : IScript
    {
        [Command("support")]
        public async Task Support_CMD(User player, params string[] args)
        {
            var playerpool = Alt.GetAllPlayers();
            
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var supportText = $"{dbChar.firstName} {dbChar.lastName} ({dbChar.playerId}) ";

            foreach (var t in args)
            {
                supportText += $"{t} ";
            }

            foreach (var x  in playerpool)
            {
                
                var tempPlayer = x as User;
                if(tempPlayer.adminLevel >= 1)
                {
                    
                    await NotifyHandler.SendSupportMessage(tempPlayer, supportText, 30000);
                    
                }
            }
        }
        [Command("packgun")]
        public async Task pack_CMD(User player)
        {
            var currentWeapon = player.CurrentWeapon;
            player.SendChatMessage(currentWeapon.ToString("x"));
            player.RemoveWeapon(currentWeapon);

            switch (currentWeapon.ToString("x").ToUpper())
            {
                case "83BF0278":
                    player.SendChatMessage("Du hast Karabiner in der Hand!");
                    break;
            }

            if(!await player.addItem(8, 1)) return;


        }
        [Command("givekey")]
        public async Task givekey_CMD(User player, string str_playerid)
        {
            int playerid = int.Parse(str_playerid);
            if (!player.Vehicle.Exists)
            {
                await NotifyHandler.SendNotification(player, "Du sitzt in keinem Auto!");
                return;
            }
            var playerVehicle = (Car)player.Vehicle;
            var dbCar = Main.database.CarCollection.AsQueryable().ToList().FirstOrDefault(c => c.carId == playerVehicle.carId);
            if (dbCar.allowedIds.Contains(playerid))
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler hat bereits einen Schlüssel!");
                return;
            }
            dbCar.allowedIds.Add(playerid);
            playerVehicle.allowedIds.Add(playerid);
            await Main.database.CarCollection.ReplaceOneAsync(c => c.carId == playerVehicle.carId, dbCar);
        }
        [Command("myid")]
        public async Task myid_CMD(User player)
        {
            var dbchar = Utils.Utils.GetDatabaseCharacter(player);
            await NotifyHandler.SendNotification(player, $"Deine ID: {dbchar.playerId}");
        }

        [Command("leave")]
        public async Task Leave(User player)
        {
            if (player.paintballArena == 0)
            {
                return;
            }
            await NotifyHandler.SendNotification(player, "Du hast Paintball verlassen!");
            await Utils.Utils.LoadClothes(player, Utils.Utils.GetDatabaseCharacter(player));
            Main.database.PaintballCollection.AsQueryable().FirstOrDefault(p => p.arenaId == player.paintballArena).playerCount -= 1;
            player.RemoveAllWeapons();
            player.paintballArena = 0;
            player.Dimension = 0;
            await Utils.Utils.LoadPlayerWeapons(player);
        }

        [Command("ooc")]
        public async Task ooc_Chat(User player, params string[] args)
        {
            var playerPool = Alt.GetAllPlayers();
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var chatMessage = "";
            foreach (var x in args)
            {
                chatMessage += $"{x} ";
            }
            foreach (User p in playerPool.Where(x => x.Position.IsInRange(player.Position, 10f)))
            {
                await NotifyHandler.SendNotification(p, $"OOC: {dbChar.firstName} {dbChar.lastName} ({dbChar.playerId}) {chatMessage}");
            }
        }

        [Command("nummer")]
        public Task getNumber(User player)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            NotifyHandler.SendNotification(player, "Deine Telefonnummer: " + dbChar.Number);
            return Task.CompletedTask;
        }

        [Command("endcall")]
        public Task endCall(User player)
        {
            var playerPool = Alt.GetAllPlayers();
            if (player.IsCalling == 0) return Task.CompletedTask;
            foreach (User p in playerPool)
            {
                if (p.playerId != player.IsCalling) continue;
                p.Emit("SaltyChat_EndCall", player.Id);
                player.Emit("SaltyChat_EndCall", p.Id);
                p.IsCalling = 0;
                player.IsCalling = 0;
            }
            return  Task.CompletedTask;
        }

        [Command("jailtime")]
        public Task jailtime_CMD(User player)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player.playerId);
            if (dbChar.jailTime > 1)
            {
                NotifyHandler.SendNotification(player, $"Du hast noch {dbChar.jailTime} Hafteinheiten!");
                return Task.CompletedTask;
            }
            NotifyHandler.SendNotification(player, $"Du befindest dich nicht im Gefängnis!");
            return Task.CompletedTask;
        }
    }
}
