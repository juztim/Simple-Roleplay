using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{
    public class NametagHandlers : IScript
    {
        private static Timer NametagTimer { get; set; } = new Timer();

        public static Task Init()
        {
            NametagTimer.Interval = 300;
            NametagTimer.Start();
            NametagTimer.Elapsed += NametagTimer_Elapsed;
            return Task.CompletedTask;
            
        }

        private static void NametagTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (User player in Alt.GetAllPlayers())
            {
                player.Emit("Server:GetAmmoCount");
            }
        }

        [ClientEvent("Server:SetAmmo")]
        public static Task SetAmmo(User player, int ammo)
        {
            player.CurrentAmmo = ammo;
            player.SetStreamSyncedMetaData("CurrentAmmo", player.CurrentAmmo);
            return Task.CompletedTask;
        }
    }
}
