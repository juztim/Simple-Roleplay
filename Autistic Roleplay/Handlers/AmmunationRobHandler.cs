using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public class AmmunationRobHandler : IScript
    {
        private static Timer ShopRobTimer { get; set; }
        private static Timer CheckRangeTimer { get; set; }
        private static AmmunitionCollection dbItemShop { get; set; }
        private static User player { get; set; }

        [ClientEvent("server:ammunation:rob")]
        public Task RobAmmunation(User x, int ammoId)
        {
            player = x;
            dbItemShop = Main.database.AmmunationCollection.AsQueryable().FirstOrDefault(s => s.Id == ammoId);
            var playerpool = Alt.GetAllPlayers();
            if (Utils.Serverglobals.IsShopRobActive)
            {
                NotifyHandler.SendNotification(player, $"Es läuft bereits ein Raub!", 10000);
                return Task.CompletedTask;
            }

            Utils.Serverglobals.IsShopRobActive = true;
            ShopRobTimer = new Timer();
            CheckRangeTimer = new Timer();

            ShopRobTimer.Elapsed += new ElapsedEventHandler(ShopRobFinished);
            ShopRobTimer.Interval = 600000;
            ShopRobTimer.Enabled = true;

            CheckRangeTimer.Elapsed += new ElapsedEventHandler(CheckRange);
            CheckRangeTimer.Interval = 1000;
            CheckRangeTimer.Enabled = true;


            NotifyHandler.SendNotification(player, "Du bist dabei den Ammunation auszurauben!");


            foreach (User p in playerpool)
            {
                if (p.frakId != 1 && p.frakId != 2) continue;
                NotifyHandler.SendNotification(p, $"Ein Ammunation wird ausgeraubt! ({dbItemShop.Name})", 10000);
            }
            return Task.CompletedTask;
        }

        private void CheckRange(object sender, ElapsedEventArgs e)
        {
            if (!player.Position.IsInRange(dbItemShop.Pos.ToAltPos(), 7f))
            {
                NotifyHandler.SendNotification(player,
                    "Du hast dich zu weit entfernt! Der Ammunationraub wurde abgebrochen!");
                var playerpool = Alt.GetAllPlayers();
                foreach (User p in playerpool)
                {
                    if (p.frakId != 1 && p.frakId != 2) continue;
                    NotifyHandler.SendNotification(p, $"Ein Ammunationraub wurde vorzeitig abgebrochen! ({dbItemShop.Name})", 10000);
                }
                Utils.Serverglobals.IsShopRobActive = false;
                ShopRobTimer.Dispose();
                CheckRangeTimer.Dispose();
            }

        }

        private void ShopRobFinished(object sender, ElapsedEventArgs e)
        {
            NotifyHandler.SendNotification(player, "Ammunationraub erfolgreich!");
            Utils.Serverglobals.IsShopRobActive = false;
            ShopRobTimer.Dispose();
            CheckRangeTimer.Dispose();

        }
    }
}
