using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AltV.Net.Data;
using AltV.Net.Enums;
using Simple_Roleplay.Interactions.Types;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public enum Gangcolors
    {
        FIB = 12,
        VAGOS = 89,
        MG13 = 140,
        LCN = 12,
        MIDNIGHT = 112,
        BALLAS = 148

    }

    public class FrakGarageHandler : IScript
    {
        [ClientEvent("client:frakgarage:ausparken")]
        public async Task FrakGarage_Ausparken(User player, string str_carId)
        {
            var dbCar = Main.database.FrakcarCollection.AsQueryable()
                .FirstOrDefault(c => c.carId == Int32.Parse(str_carId));

            var ausparkPunkte = Main.database.FrakGarageCollection.AsQueryable()
                .FirstOrDefault(g => g.Id == dbCar.garageId).Ausparkpunkte;

            var dbModifiers = Main.database.VehicleModiferCollection.AsQueryable()
                .FirstOrDefault(c => c.ModelName.ToLower() == dbCar.carmodel.ToLower());

            if (!dbCar.parkedIn) return;

            for (int i = 0; i < ausparkPunkte.Count; i++)
            {
                var ausparkPunkt = ATMS.atmInteraction.toAltPos(ausparkPunkte.ElementAt(i).position);
                var ausparkRotation = ausparkPunkte.ElementAt(i).rotation;
                var ausparkBlocked = Alt.GetAllVehicles().ToList().FirstOrDefault(v => v.Position.IsInRange(ausparkPunkt, 1.5f));
                if (ausparkBlocked != null)
                {

                }
                else
                {
                    Car spawnCar = (Car)Alt.CreateVehicle(dbCar.carmodel, ausparkPunkt, new Rotation(0, 0, (float)ausparkRotation));
                    switch (dbCar.frakId)
                    {
                        case 2:
                            spawnCar.PrimaryColor = (byte) Gangcolors.FIB;
                            spawnCar.SecondaryColor = (byte)Gangcolors.FIB;
                            break;
                        case 5:
                            spawnCar.PrimaryColor = (byte) Gangcolors.MIDNIGHT;
                            spawnCar.SecondaryColor = (byte)Gangcolors.MIDNIGHT;
                            break;
                        case 7:
                            spawnCar.PrimaryColor = (byte) Gangcolors.MG13;
                            spawnCar.SecondaryColor = (byte) Gangcolors.MG13;
                            break;
                        case 8:
                            spawnCar.PrimaryColor = (byte) Gangcolors.LCN;
                            spawnCar.SecondaryColor = (byte) Gangcolors.LCN;
                            break;
                        case 9:
                            spawnCar.PrimaryColor = (byte) Gangcolors.VAGOS;
                            spawnCar.SecondaryColor = (byte) Gangcolors.VAGOS;
                            break;
                        case 10:
                            spawnCar.PrimaryColor = (byte) Gangcolors.BALLAS;
                            spawnCar.SecondaryColor = (byte) Gangcolors.BALLAS;
                            break;
                    }

                    spawnCar.NumberplateText = dbCar.numPlate;
                    spawnCar.frakId = dbCar.frakId;
                    spawnCar.numPlate = dbCar.numPlate;
                    spawnCar.fuel = dbCar.fuel;
                    spawnCar.kilometer = dbCar.kilometer;
                    spawnCar.carId = dbCar.carId;
                    spawnCar.ManualEngineControl = true;
                    spawnCar.carmodel = dbCar.carmodel;
                    spawnCar.LockState = VehicleLockState.Locked;

                    dbCar.parkedIn = false;
                    Main.database.FrakcarCollection.ReplaceOne(c => c.carId == dbCar.carId, dbCar);
                    await Task.Delay(500);
                    Alt.EmitAllClients("vehicle:setSpeed", spawnCar, dbModifiers.VehSpeedModifier);
                    break;
                }
                if (i == ausparkPunkte.Count && ausparkBlocked.Exists)
                {
                    await NotifyHandler.SendNotification(player, "Es sind keine Ausparkpunkte frei!");
                    break;
                }
            }
            return;
        }

        [ClientEvent("client:frakgarage:einparken")]
        public Task FrakGarage_Einparken(User player, string str_carid)
        {
            var dbCar = Main.database.FrakcarCollection.AsQueryable()
                .FirstOrDefault(c => c.carId == int.Parse(str_carid));

            foreach (Car vehicle in Alt.GetAllVehicles())
            {
                if(dbCar.carId != vehicle.carId) continue;

                dbCar.parkedIn = true;
                dbCar.fuel = vehicle.fuel;
                dbCar.kilometer = vehicle.kilometer;
                vehicle.Remove();
            }

            Main.database.FrakcarCollection.ReplaceOne(c => c.carId == dbCar.carId, dbCar);

            


            return Task.CompletedTask;
        }
    }
    
}

