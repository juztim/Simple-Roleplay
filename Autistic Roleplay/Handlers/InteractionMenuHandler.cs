using System;
using System.Collections.Generic;
using System.Linq;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Simple_Roleplay.Factories;
using System.Threading.Tasks;
using AltV.Net.Data;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple_Roleplay.Utils;
using IMongoCollectionExtensions = MongoDB.Driver.IMongoCollectionExtensions;

namespace Simple_Roleplay.Handlers
{
    public class InteractionMenuHandler : IScript
    {
        [ClientEvent("Server:Interaction:GetPlayerInfo")]
        public Task GetPlayerInfo (User player, User requestedPlayer)
        {
            if (player == null || !player.Exists || player.playerId <= 0 || requestedPlayer == null || !requestedPlayer.Exists || requestedPlayer.playerId <= 0) return Task.CompletedTask;

            string interactHTML = "";
            interactHTML += "<li><p id='InteractionMenu-Title'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Schließen'><img src='img/interact/close.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-showIdCard' data-action='showIDCard' data-actionstring='Personalausweis Zeigen'><img src='img/interact/idcard.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-giveMoney' data-action='giveMoney' data-actionstring='Geld geben'><img src='img/interact/giveMoney.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-fesseln' data-action='fesseln' data-actionstring='Fesseln'><img src='img/interact/seil.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-showLicense' data-action='showLicenses' data-actionstring='Führerschein Zeigen'><img src='img/interact/license.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-supportInfo' data-action='supportInfo' data-actionstring='Support Information'><img src='img/interact/info.png'></li>";
            //Check if player is medic, is on duty and has defibrillator
            if (player.frakId == 8 && player.IsJobDuty)
            {
                interactHTML += "<li class='interactitem' id='InteractionMenu-reanimate' data-action='reanimate' data-actionstring='Reanimieren'><img src='img/interact/reanimate.png'></li>";
                interactHTML += "<li class='interactitem' id='InteractionMenu-reanimate' data-action='healPlayer' data-actionstring='Spieler Behandeln'><img src='img/interact/healPlayer.png'></li>";
            }

            if (player.frakId == 1 || player.frakId == 2 && player.Position.IsInRange(new Position(1690.8704833984375f, 2591.9072265625f, 45.84127426147461f), 3f))
            {
                interactHTML += "<li class='interactitem' id='InteractionMenu-jail' data-action='jailPlayer' data-actionstring='Inhaftieren'><img src='img/interact/police-handcuffs.png'></li>";
            }
            player.Emit("Client:Interaction:SetInfo", "player", interactHTML);

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:GetVehicleInfo")]
        public Task GetVehicleInfo(User player, string type, Car vehicle)
        {
            if (player == null || !player.Exists || player.playerId <= 0 || vehicle == null || !vehicle.Exists || vehicle.carId <= 0) return Task.CompletedTask;

            string interactHTML = "";
            interactHTML += "<li><p id='InteractionMenu-Title'>Schließen</p></li><li class='interactitem' data-action='close' data-actionstring='Schließen'><img src='img/interact/close.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-lockVehicle' data-action='lockVehicle' data-actionstring='Fahrzeug auf- / abschließen'><img src='img/interact/lockVehicle.png'></li>";
            interactHTML += "<li class='interactitem' id='InteractionMenu-openTrunk' data-action='openTrunk' data-actionstring='Kofferraum Öffnen / Schließen'><img src='img/interact/openTrunk.png'></li>";

            if (type.ToLower() == "vehiclein")
            {
                if (player.Seat == 1)
                {
                    interactHTML += "<li class='interactitem' id='InteractionMenu-toggleEngine' data-action='toggleEngine' data-actionstring='Motor An/Aus'><img src = 'img/interact/toggleEngine.png' ></ li >";
                }
            }

            if (type.ToLower() == "vehicleout")
            {
                if (player.HasItem(28))
                {
                    interactHTML += "<li class='interactitem' id='InteractionMenu-repairCar' data-action='repairCar' data-actionstring='Reparieren'><img src = 'img/interact/repair.png' ></ li >";
                }
            }

            if (player.isAduty)
            {
                interactHTML += "<li class='interactitem' id='InteractionMenu-supportInfo' data-action='supportInfo' data-actionstring='Support Information'><img src='img/interact/info.png'></li>";
            }
            player.Emit("Client:Interaction:SetInfo", type, interactHTML);

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:lockVehicle")]
        public Task LockVehicle(User player, Car vehicle)
        {
            if (player == null || !player.Exists || player.playerId <= 0 || vehicle == null || !vehicle.Exists || vehicle.carId <= 0) return Task.CompletedTask;
            Console.WriteLine("64");
            if (vehicle.frakId > 0)
            {
                if (vehicle.frakId != player.frakId)
                {
                    NotifyHandler.SendNotification(player, "Du hast keinen Schlüssel!");
                    return Task.CompletedTask;
                }
            }
            else
            {
                if (!vehicle.allowedIds.Contains(player.playerId) && vehicle.ownerId != player.playerId)
                {
                    Console.WriteLine("67");

                    NotifyHandler.SendNotification(player, "Du hast keinen Schlüssel!");
                    return Task.CompletedTask;
                }
            }
           

            if (vehicle.LockState == VehicleLockState.Locked)
            {
                NotifyHandler.SendNotification(player, "Fahrzeug aufgeschlossen!");
                vehicle.LockState = VehicleLockState.Unlocked;
                return Task.CompletedTask;
            }
            if (vehicle.LockState == VehicleLockState.Unlocked)
            {
                NotifyHandler.SendNotification(player, "Fahrzeug abgeschlossen!");
                vehicle.LockState = VehicleLockState.Locked;
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:toggleEngine")]
        public Task ToggleEngine(User player, Car vehicle)
        {
            if (player == null || !player.Exists || player.playerId <= 0 || vehicle == null || !vehicle.Exists || vehicle.carId <= 0) return Task.CompletedTask;
            if (vehicle.frakId > 0)
            {
                if (vehicle.frakId != player.frakId)
                {
                    NotifyHandler.SendNotification(player, "Du hast keinen Schlüssel!");
                    return Task.CompletedTask;
                }
            }
            else
            {
                if (!vehicle.allowedIds.Contains(player.playerId) && vehicle.ownerId != player.playerId)
                {
                    Console.WriteLine("67");

                    NotifyHandler.SendNotification(player, "Du hast keinen Schlüssel!");
                    return Task.CompletedTask;
                }
            }

            if (vehicle.EngineOn)
            {
                NotifyHandler.SendNotification(player, "Motor Aus");
                vehicle.EngineOn = false;
                return Task.CompletedTask;
            }

            if (!vehicle.EngineOn)
            {
                NotifyHandler.SendNotification(player, "Motor An");
                vehicle.EngineOn = true;
                return Task.CompletedTask;
                
            }

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:repairVehicle")]
        public async Task RepairVehicle (User player, Car vehicle)
        {
            player.Emit("client:playAnimation", "missmechanic", "work2_base", 4500, 1);
            await Task.Delay(4500);
            Alt.EmitAllClients("AllClients:Vehicle:Repair", vehicle);
            return;
        }

        [ClientEvent("Server:Interaction:showSupportInfo")]
        public Task ShowSupportInfo(User player, User otherPlayer)
        {
            NotifyHandler.SendNotification(player, $"Support ID: {otherPlayer.playerId}", 5000);
            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:showIDCard")]
        public Task ShowIdCard(User player, User otherplayer)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            player.Emit("client:interaction:showIdCard", dbChar.firstName, dbChar.lastName, dbChar.birthDay);
            otherplayer.Emit("client:interaction:showIdCard", dbChar.firstName, dbChar.lastName, dbChar.birthDay);
            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:showLicenses")]
        public Task ShowLicenses(User player, User otherPlayer)
        {
            var dbchar = Utils.Utils.GetDatabaseCharacter(player);
            player.Emit("client:interaction:showLicenses", dbchar.firstName, dbchar.lastName, dbchar.birthDay, "A, B, C");
            otherPlayer.Emit("client:interaction:showLicenses", dbchar.firstName, dbchar.lastName, dbchar.birthDay, "A, B, C");
            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:openTrunk")]
        public Task OpenTrunk(User player, Car vehicle)
        {
            Console.WriteLine(vehicle.GetDoorState(5));
            player.EmitLocked("Client:Interaction:openTrunk", vehicle, vehicle.GetDoorState(5) == 0);

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:TieUp")]
        public Task TieUp(User player, User otherPlayer)
        {

            return Task.CompletedTask;
        }

        [ClientEvent("Server:Interaction:reanimate")]
        public async Task Reanimate_Player(User player, User otherPlayer)
        {
            player.Emit("client:playAnimation", "missheistfbi3b_ig8_2", "cpr_loop_paramedic", 12000, 1);
            await Task.Delay(5000);
            await NotifyHandler.SendNotification(otherPlayer, "Du wurdest wiederbelebt!", 3000);
            otherPlayer.Spawn(otherPlayer.Position, 0);
            otherPlayer.Health = 110;
            return;
        }

        [ClientEvent("Server:Interaction:healPlayer")]
        public async Task Heal_Player(User player, User otherPlayer)
        {
            await NotifyHandler.SendNotification(otherPlayer, "Du wurdest behandelt!", 3000);
            otherPlayer.Health += 40;
            return;
        }

        [ClientEvent("Server:Interaction:jailPlayer")]
        public async Task jail_Player(User player, User otherPlayer)
        {

            var targetChar = IMongoCollectionExtensions.AsQueryable(Main.database.CharacterCollection)
                .FirstOrDefault(c => c.playerId == otherPlayer.playerId);

            var targetJailTime = 0;
            var targetFine = 0;

            foreach (var akte in targetChar.Akten)
            {
                var dbAkte = Main.database.FileCollection.AsQueryable().FirstOrDefault(x => x.Akten.Any(y => y.AktenId == akte)).Akten.FirstOrDefault(a => a.AktenId == akte);
                targetJailTime += dbAkte.JailTime;
                targetFine += dbAkte.Fine;
            }
            
            targetChar.Akten = new List<int>();
            otherPlayer.Position = new Position(1729.25f, 2563.642578125f, 45.56489944458008f);
            otherPlayer.Rotation = new Rotation(0,0, 174.47164916992188f);
            
            targetChar.jailTime = targetJailTime;
            NotifyHandler.SendNotification(otherPlayer,
                $"Du befindest dich nun für {targetChar.jailTime} Hafteinheiten im Gefängnis!");
            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == otherPlayer.playerId, targetChar);
            
            return;
        }
    }
}
