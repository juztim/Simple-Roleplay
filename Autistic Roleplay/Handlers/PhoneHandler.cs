using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Newtonsoft.Json;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Interactions.Types;

namespace Simple_Roleplay.Handlers
{
    public class PhoneHandler : IScript
    {
        [ClientEvent("server:openPhone")]
        public Task openPhone(User player)
        {
            //Check if player has phone
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var invPhone = dbChar.Inventar.ToList().FirstOrDefault(i => i.itemId == 30);
            if (invPhone == null)
            {
                NotifyHandler.SendNotification(player, "Du besitzt kein Smartphone!");
                return Task.CompletedTask;
            }

            if (player.frakId == 1 || player.frakId == 2)
            {
                player.Emit("client:playAnimation", "anim@cellphone@in_car@ds", "cellphone_text_read_base", -1, 51);
                player.Emit("client:openPhone", true);
                return Task.CompletedTask;
            }
            player.Emit("client:playAnimation", "anim@cellphone@in_car@ds", "cellphone_text_read_base", -1, 51);
            player.Emit("client:openPhone", false);
            

            return Task.CompletedTask;
        }

        [ClientEvent("server:closePhone")]
        public Task closePhone(User player)
        {
            player.Emit("client:closePhone");
            return Task.CompletedTask;
        }

        [ClientEvent("server:phone:openbanking")]
        public Task openBanking(User player)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var dbKonto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.objectOwnerId == dbChar.id);
            player.Emit("client:phone:openbanking", dbChar.firstName, dbChar.lastName, dbKonto.kontoStand);
            return Task.CompletedTask;
        }

        [ClientEvent("server:phone:opengarage")]
        public Task openGarage(User player)
        {
            var validcars = Main.database.CarCollection.AsQueryable()
                .Where(c => c.ownerId == player.playerId || c.allowedIds.Contains(player.playerId)).ToList();
            var carList = new List<AppCar>();

            foreach (var car in validcars.Where(c => !c.parkedIn))
            {
                var dbGarage = Main.database.GarageCollection.AsQueryable()
                    .FirstOrDefault(g => g.garageId == car.garageId);
                var dbOwner = Main.database.CharacterCollection.AsQueryable()
                    .FirstOrDefault(o => o.playerId == car.ownerId);

                foreach (Car veh in Alt.GetAllVehicles())
                {
                    if (veh.carId <= 0) continue;
                    if (veh.carId == car.carId)
                    {
                        var newCar = new AppCar
                        {
                            garageId = dbGarage.Name,
                            numPlate = car.numPlate,
                            ownerId = dbOwner.firstName + " " + dbOwner.lastName,
                            carmodel = car.carmodel,
                            pos = new Pos(veh.Position.X, veh.Position.Y, veh.Position.Z),
                            parkedIn = car.parkedIn
                        };
                        carList.Add(newCar);
                    }
                }
            }

            foreach (var car in validcars.Where(c => c.parkedIn))
            {
                var dbGarage = Main.database.GarageCollection.AsQueryable()
                    .FirstOrDefault(g => g.garageId == car.garageId);
                var dbOwner = Main.database.CharacterCollection.AsQueryable()
                    .FirstOrDefault(o => o.playerId == car.ownerId);
                var newCar = new AppCar
                {
                    garageId = dbGarage.Name,
                    numPlate = car.numPlate,
                    ownerId = dbOwner.firstName + " " + dbOwner.lastName,
                    carmodel = car.carmodel,
                    parkedIn = car.parkedIn
                };
                carList.Add(newCar);

            }




            var carjson = JsonConvert.SerializeObject(carList);
            player.Emit("client:phone:opengarage", carjson);
            return Task.CompletedTask;
        }

        [ClientEvent("server:phone:openteamapp")]
        public Task openTeam(User player)
        {
            var dbplayerfrak = Main.database.FractionCollection.AsQueryable()
                .FirstOrDefault(f => f.frakId == player.frakId);

            var frakmember = new List<AppMember>();


            foreach (var member in dbplayerfrak.frakMember)
            {
                var dbMember = Main.database.CharacterCollection.AsQueryable()
                    .FirstOrDefault(c => c.playerId == member.playerid);
                if (!dbMember.isOnline) continue;
                var newfrakmember = new AppMember
                    {firstName = dbMember.firstName, lastName = dbMember.lastName, rank = member.rank};
                frakmember.Add(newfrakmember);
            }

            if (dbplayerfrak != null)
            {
                var sortedList = frakmember.OrderByDescending(f => f.rank).ToList();

                player.Emit("client:phone:openteamapp", JsonConvert.SerializeObject(sortedList));
                return Task.CompletedTask;
            }

            player.Emit("client:phone:openteamapp", "nofrak");
            return Task.CompletedTask;
        }

        [ClientEvent("server:phone:startCall")]
        public Task StartCall(User player, string str_number)
        {
            int number = Int32.Parse(str_number);
            var otherChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.Number == number);
            if (otherChar == null)
            {
                player.Emit("client:phone:notExisting");
                return Task.CompletedTask;
            }

            var playerPool = Alt.GetAllPlayers();

            if (!otherChar.isOnline)
            {
                player.Emit("client:phone:notOnline");
                return Task.CompletedTask;
            }

            foreach (User p in playerPool)
            {
                if (p.playerId != otherChar.playerId) continue;
                if (p.IsCalling > 0 || p.Equals(player))
                {
                    player.Emit("client:phone:busy");
                    return Task.CompletedTask;
                }

                p.Emit("client:phone:incomingCall", Utils.Utils.GetDatabaseCharacter(player).Number);
                p.IsCalling = player.playerId;
                player.IsCalling = p.playerId;
            }

            player.Emit("client:phone:startCall", otherChar.Number);



            return Task.CompletedTask;

        }

        [ClientEvent("server:phone:acceptIncoming")]
        public Task AcceptIncomingCall(User player)
        {
            var otherChar = Main.database.CharacterCollection.AsQueryable()
                .FirstOrDefault(c => c.playerId == player.IsCalling);


            var playerPool = Alt.GetAllPlayers();
            foreach (User p in playerPool)
            {
                if (p.playerId != otherChar.playerId) continue;
                if (p.playerId == otherChar.playerId)
                {
                    p.Emit("SaltyChat_EstablishedCall", player);
                    player.Emit("SaltyChat_EstablishedCall", p);
                    break;
                }

            }

            return Task.CompletedTask;

        }

        [ClientEvent("server:phone:denyIncoming")]
        public Task DenyIncomingCall(User player)
        {
            var playerpool = Alt.GetAllPlayers();


            foreach (User p in playerpool)
            {
                if (p.IsCalling == player.playerId)
                {
                    p.IsCalling = 0;
                    p.Emit("server:phone:denyCall");
                    player.IsCalling = 0;
                    break;
                }
            }

            return Task.CompletedTask;
        }

        [ClientEvent("server:phone:auflegen")]
        public Task AuflegenPhone(User player)
        {

            var playerpool = Alt.GetAllPlayers();


            foreach (User p in playerpool)
            {
                if (p.IsCalling == player.playerId)
                {
                    p.IsCalling = 0;
                    p.Emit("server:phone:hangUpCall");
                    player.IsCalling = 0;
                    break;
                }
            }


            return Task.CompletedTask;
        }

        [ClientEvent("server:contacts:requestAll")]
        public Task RequestContacts(User player)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player.playerId);
            var contactList = dbChar.Kontakte;
            player.Emit("server:phone:sendContacts", JsonConvert.SerializeObject(contactList));
            return Task.CompletedTask;
        }

        [ClientEvent("server:contacts:add")]
        public Task AddContact(User player, string name, string number)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player.playerId);
            var newContact = new Contact {Name = name, phoneNumber = int.Parse(number)};
            dbChar.Kontakte.Add(newContact);
            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbChar);
            return Task.CompletedTask;
        }
    }
}

