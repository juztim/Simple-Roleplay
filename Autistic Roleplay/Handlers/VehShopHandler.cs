using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using System.Linq;
using System.Net.Http.Headers;
using AltV.Net.Data;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public class VehShopHandler : IScript
    {
        [ClientEvent("server:cardealer:buycar")]
        public Task BuyCar(User player, int dealerId, string carId)
        {
            var nextCarId = Main.database.CarCollection.AsQueryable().ToList().Count + 1;
            var dbDealer = Main.database.VehShopCollection.AsQueryable().FirstOrDefault(v => v.Id == dealerId);
            var dbDealerCar = dbDealer.Cars.ToList().ElementAt(Int32.Parse(carId));
            var numPlate = "";
            var numPlateExists = false;

            var isPunktBlocked = Alt.GetAllVehicles().ToList().FirstOrDefault(v => v.Position.IsInRange(dbDealer.SpawnPosition.ToAltPos(), 1.5f));

            if (isPunktBlocked != null)
            {
                NotifyHandler.SendNotification(player,"Der Ausparkpunkt ist zurzeit blockiert!");
                return Task.CompletedTask;
            }

            do
            {
                var random = new Random();
                var randomNumber = random.Next(10000, 99999);

                numPlate = "CAR" + randomNumber;

                var dbNumPlate = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.numPlate == numPlate);

                if (dbNumPlate != null) numPlateExists = true;

            } while (numPlateExists);

            var newCar = new Carcollection()
            {
                allowedIds = new List<int>(), parkedIn = false, carId = nextCarId, carmodel = dbDealerCar.Model,
                fuel = 100, fuelType = 1, garageId = 1, kilometer = 0, numPlate = numPlate, ownerId = player.playerId
            };
            Main.database.CarCollection.InsertOne(newCar);
            var spawnVeh = (Car)Alt.CreateVehicle(dbDealerCar.Model, dbDealer.SpawnPosition.ToAltPos(), dbDealer.SpawnRotation.ToAltPos());
            spawnVeh.numPlate = newCar.numPlate;
            spawnVeh.carmodel = newCar.carmodel;
            spawnVeh.ownerId = newCar.ownerId;
            spawnVeh.NumberplateText = numPlate;
            spawnVeh.allowedIds = new List<int>();
            spawnVeh.fuel = 100f;
            spawnVeh.kilometer = 0;
            spawnVeh.carId = newCar.carId;
            spawnVeh.ManualEngineControl = true;
            spawnVeh.EngineOn = false;
            spawnVeh.LockState = VehicleLockState.Locked;

            return Task.CompletedTask;
        }
    }
}
