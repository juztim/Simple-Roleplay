using System;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Events
{
    public class CarEvents : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public void EnterCar_Event(Car vehicle, IPlayer player, byte seat)
        {

            player.Emit("client:hud:showCarHud", vehicle.kilometer, vehicle.fuel);
        }
        [ScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public void LeaveCar_Event(Car vehicle, IPlayer player, byte seat)
        {
            player.Emit("client:hud:closeCarHud");
        }
        [ClientEvent("client:cef:hud:car:updateStats")]
        public void updateKilometer_Event(User player, int kilometer, double spritverbrauch)
        {
            var vehicle = (Car)player.Vehicle;
            vehicle.fuel -= spritverbrauch;
            vehicle.kilometer += kilometer;
        }

        [ClientEvent("server:car:muteSiren")]
        public Task muteSiren_Event(User player, Car veh)
        {
            if(player.Seat != 1) return Task.CompletedTask;
            bool sirenstate;
            
            veh.GetStreamSyncedMetaData("sirenState", out sirenstate);
            Console.WriteLine($"Sirenstate: {sirenstate}");
            veh.SetStreamSyncedMetaData("sirenState", !sirenstate);
            return Task.CompletedTask;
        }
    }
}
