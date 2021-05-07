using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Async.Events;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Handlers;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Events
{
    public class ServerEvents : IScript
    {

        [ScriptEvent(ScriptEventType.ColShape)]
        public Task OnColShape(IColShape colShape, IEntity entity, bool state)
        {
            if (state == false) return Task.CompletedTask;
            if (entity is IPlayer)
            {
                var player = entity as User;
                if (!player.IsInVehicle) return Task.CompletedTask;
                NotifyHandler.SendNotification(player, "Verlasse dieses Gebiet innerhalb von 10 Sekunden", 10000);
            }

            if (entity is IVehicle)
            {
                var veh = entity as Car;
                Console.WriteLine("Create Timer");
                var EmpTimer = new Timer();
                EmpTimer.Interval = 10000;
                EmpTimer.Enabled = true;
                EmpTimer.Elapsed += (sender, args) =>
                {
                    Console.WriteLine("Timer elapsed");

                    if (!veh.Position.IsInRange(colShape.Position, 200f))
                    {
                        EmpTimer.Dispose();
                        return;
                       
                    };
                    Console.WriteLine("Veh inside!");
                    EmpTimer.Dispose();
                    veh.SetNetworkOwner(veh.Driver, false);
                    veh.Driver.Emit("NetOwner:DestroyVehicle", veh);

                };
            }
            return Task.CompletedTask;
        }

    }
}
