using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using System.Threading.Tasks;

namespace Simple_Roleplay.Handlers
{
    public class FuelStationHandler : IScript
    {
        [ClientEvent("server:tankstelle:tanken")]
        public async Task tanken(User player, string str_carId, string str_fuelAmount)
        {
            int carId = int.Parse(str_carId);
            int fuelAmount = int.Parse(str_fuelAmount);
            
            
            //Auto in DB finden
            var dbCar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.carId == carId);
            //Sprit setzen
            dbCar.fuel += fuelAmount;
            if (dbCar.fuel > 100.00) dbCar.fuel = 100.00;
            //Auto updaten in DB
            await Main.database.CarCollection.ReplaceOneAsync(c => c.carId == carId, dbCar);

            //Auto auf Server finden
            foreach (Car car in Alt.GetAllVehicles())
            {
                if(car.carId == carId)
                {
                    car.fuel += fuelAmount;
                    break;
                }
            }
            await NotifyHandler.SendNotification(player, "Fahrzeug getankt! Tankfüllung: " + dbCar.fuel + "l", 3000);
        }
    }
}
