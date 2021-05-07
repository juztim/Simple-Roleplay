using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Resources.Chat.Api;
using System;
using Simple_Roleplay.Utils;
using Simple_Roleplay.Handlers;

namespace Simple_Roleplay.Commands
{
    class Adutycommands : IScript
    {
        [Command("veh")]
        public void veh_CMD(User player, string vehName)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var dbUser = Utils.Utils.GetDatabasePlayer(player);
            if (!player.isAduty) return;
            Car spawnedVeh = (Car)Alt.CreateVehicle(vehName.ToLower(), player.Position
                , new Rotation(0, 0, 0));
            Discord.Webhookhandler.sendMessage($"[{dbChar.firstName} {dbChar.lastName} / {dbUser.Username}] Spawn: {vehName}. `{DateTime.Now.ToLocalTime():dd.MM.yyyy HH:mm:ss}`");
            spawnedVeh.EngineOn = true;
            spawnedVeh.NumberplateText = "Admin";
            spawnedVeh.fuel = 100.00;
            spawnedVeh.isAdminCar = true;
        }

        [Command("weapon")]
        public void weapon_CMD(User player, string weaponName)
        {
            if (!player.isAduty) return;
            var weaponHash = Alt.Hash(weaponName);
            player.GiveWeapon(weaponHash, 9999, true);
        }

        [Command("tp")]
        public void weapon_CMD(User player, float posX, float posY, float posZ)
        {
            if (!player.isAduty) return;
            var pos = new Position(posX, posY, posZ);
            player.Position = pos;
        }

        [Command("aduty")]
        public async Task aduty_CMDAsync(User player)
        {
            Users dbUser = Main.database.UserCollection.AsQueryable().FirstOrDefault(x => x.Username == player.Username);
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(x => x.ownerObjId == dbUser.id);

            if (player.adminLevel == 0)
            {
                await NotifyHandler.SendNotification(player, "Keine Rechte!");
                return;
            }

            if (player.isAduty)
            {
                player.Emit("player:aduty:leave");
                await Utils.Utils.LoadClothes(player, dbChar);
                Discord.Webhookhandler.sendMessage($"[{dbChar.firstName} {dbChar.lastName} / {dbUser.Username}] Aduty verlassen. `{DateTime.Now.ToLocalTime():dd.MM.yyyy HH:mm:ss}`");
                player.isAduty = false;
                return;
            }
            if (!player.isAduty)
            {

                player.Emit("player:aduty:enter", player.sex, player.adminLevel);
                Discord.Webhookhandler.sendMessage($"[{dbChar.firstName} {dbChar.lastName} / {dbUser.Username}] Aduty betreten. `{DateTime.Now.ToLocalTime():dd.MM.yyyy HH:mm:ss}`");
                player.isAduty = true;
            }
        }
        [Command("dv")]
        public void DV_CMD(User player)
        {
            if (!player.isAduty) return;
            if (player.Vehicle.Exists)
            {
                player.Vehicle.Remove();
            }
        }
        [Command("speed")]
        public Task Speed_CMD(User player, int speed)
        {
            if (!player.Vehicle.Exists) return Task.CompletedTask;
            player.Emit("player:aduty:setvehspeed", speed);
            return Task.CompletedTask;
        }

        [Command("anim")]
        public async Task anim_CMD(User player, string animdict, string anim)
        {
            player.Emit("client:playAnim", animdict, anim);
        }

        [Command("goto")]
        public Task goto_CMD(User player, int wantedId)
        {
            if (wantedId <= 0) return Task.CompletedTask;
            if (!player.isAduty) return Task.CompletedTask;
            player.Position = Utils.Utils.FindPlayer(wantedId).Position;
            return Task.CompletedTask;

        }

        [Command("gethere")]
        public Task gethere_CMD(User player, int wantedId)
        {
            if (!player.isAduty) return Task.CompletedTask;
            Utils.Utils.FindPlayer(wantedId).Position = player.Position;
            return Task.CompletedTask;
        }

        [Command("kick")]
        public Task kick_CMD(User player, int wantedId, params string[] args)
        {
            if (!player.isAduty) return Task.CompletedTask;
            var kickReason = "";
            foreach (var t in args)
            {
                kickReason += $" {t}";
            }
            Utils.Utils.FindPlayer(wantedId).Kick(kickReason);
            return Task.CompletedTask;
        }

        [Command("parkin")]
        public async Task parkin_CMD(User player, string str_carId)
        {
            var carId = int.Parse(str_carId);
            if (!player.isAduty) return;
            var dbcar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.carId == carId);
            if (dbcar == null) return;
            var carpool = Alt.GetAllVehicles();
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Car car in carpool)
            {
                if (car.carId == carId)
                {
                    car.Remove();
                    break;
                }
            }
            dbcar.parkedIn = true;
            await Main.database.CarCollection.ReplaceOneAsync(c => c.carId == carId, dbcar);
        }

        [Command("carinfo")]
        public async Task carinfo_CMD(User player)
        {
            var carinrange = (Car)Alt.GetAllVehicles().FirstOrDefault(x => x.Position.IsInRange(player.Position, 1.5f));
            if (!carinrange.Exists) return;
            var dbCar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.carId == carinrange.carId);
            var dbcarOwner = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.playerId == dbCar.ownerId);
            await NotifyHandler.SendNotification(player, $"Model: {dbCar.carmodel} CarID: {dbCar.carId} Owner: {dbcarOwner.firstName} {dbcarOwner.lastName}");
        }
        [Command("abike")]
        public Task abike_CMD(User player)
        {
            if (!player.isAduty) return Task.CompletedTask;

            dynamic spawnedVeh;
            switch (player.adminLevel)
            {
                case 4:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 255, 0, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 255, 0, 255);
                    break;
                case 5:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 0, 255, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 0, 255, 255);
                    break;
                case 6:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 255, 230, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 255, 230, 255);
                    break;
                case 7:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(59, 0, 179, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(59, 0, 179, 255);
                    break;
                case 8:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(255, 0, 0, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(255, 0, 0, 255);
                    break;
                case 9:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Shotaro, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 0, 0, 0);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 0, 0, 0);
                    break;
            }

            return Task.CompletedTask;
        }
        [Command("abike2")]
        public Task abiketwo_CMD(User player)
        {
            if (!player.isAduty) return Task.CompletedTask;

            dynamic spawnedVeh;
            switch (player.adminLevel)
            {
                case 5:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Oppressor2, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 0, 255, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 0, 255, 255);
                    break;
                case 6:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Oppressor2, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 255, 255, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 255, 255, 255);
                    break;
                case 7:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Oppressor2, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(59, 0, 179, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(59, 0, 179, 255);
                    break;
                case 8:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Oppressor2, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(255, 0, 0, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(255, 0, 0, 255);
                    break;
                case 9:
                    spawnedVeh = Alt.CreateVehicle(VehicleModel.Oppressor2, player.Position, new Rotation(0, 0, 0));
                    spawnedVeh.PrimaryColorRgb = new Rgba(0, 0, 0, 255);
                    spawnedVeh.SecondaryColorRgb = new Rgba(0, 0, 0, 255);
                    break;
                
            }
            return Task.CompletedTask;
        }
        [Command("rot")]
        public Task rot_CMD(User player)
        {
            if (!player.isAduty) return Task.CompletedTask;
            if (!player.IsInVehicle) return Task.CompletedTask;

            Console.WriteLine(player.Vehicle.Rotation.ToString());
            return Task.CompletedTask;
        }

        [Command("call")]
        public Task call_CMD(User player, string otherId)
        {
            foreach (User p in Alt.GetAllPlayers())
            {
                if (p.playerId == Int32.Parse(otherId))
                {
                    player.Emit("SaltyChat_EstablishedCall", p);
                    p.Emit("SaltyChat_EstablishedCall", player);
                    break;
                }
            }
            return Task.CompletedTask;
        }



        [Command("Scenario")]
        public Task scenario_CMD(User player, string scenario)
        {
            player.Emit("client:playScenario", scenario);
            return Task.CompletedTask;
        }

        [Command("announce")]
        public Task announce_CMD(User player, params string[] args)
        {
            
            return Task.CompletedTask;
        }

        [Command("vehspeed")]
        public Task vehspeed_CMD(User player, int speed)
        {
            player.Emit("setVehSpeed", speed);
            return Task.CompletedTask;
        }

        [Command("topspeed")]
        public Task topspeed_CMD(User player, int speed)
        {
            player.Emit("setTopSpeed", speed);
            return Task.CompletedTask;
        }

        [Command("model")]
        public Task changeGenderF_CMD(User player, string model)
        {
            player.Model = Alt.Hash(model);
            return Task.CompletedTask;
        }

        [Command("medic")]
        public async Task medicDuty_CMD(User player)
        {
            Console.WriteLine("Call Medic CMD" + "Jobduty: " + player.IsJobDuty);
            if (player.IsJobDuty) 
            {
                await NotifyHandler.SendNotification(player, "Du bist nun nichtmehr im Medic-Dienst!");
                player.IsJobDuty = false;
                return;
            }
            else
            {
                await NotifyHandler.SendNotification(player, "Du bist nun im Medic-Dienst!");
                player.IsJobDuty = true;
                return;
            }
        }   

        [Command("pwset")]
        public async Task pwset_CMD(User player, string username, string password)
        {
            if (!player.isAduty) return;
            var dbUser = Utils.Utils.GetDatabasePlayer(player);
            if (dbUser.adminLevel != 9) return;
            var wantedUser = Main.database.UserCollection.AsQueryable().FirstOrDefault(u => u.Username.ToLower() == username.ToLower());
            if (wantedUser == null) return;
            wantedUser.Password = BCrypt.Net.BCrypt.HashString(password);
            Main.database.UserCollection.ReplaceOne(u => u.Username.ToLower() == username.ToLower(), wantedUser);
            await NotifyHandler.SendNotification(player, "Du hast das Passwort von " + username + " erfolgreich geändert!");
        }

        [Command("notify")]
        public Task notify_CMD(User player, string message)
        {
            player.Emit("react:notify:show", message);
            return Task.CompletedTask;
        }

        [Command("revive")]
        public Task revive_CMD(User player, int targetId)
        {
            if (!player.isAduty || player.adminLevel < 5) return Task.CompletedTask;
            var playerpool = Alt.GetAllPlayers();
            foreach (User p in playerpool)
            {
                if(p.playerId != targetId) continue;
                p.Spawn(p.Position, 0);
                NotifyHandler.SendNotification(p, "Du wurdest von einem Admin wiederbelebt!");
            }
           
            
            return Task.CompletedTask;
        }
        
        
        


    }
}
