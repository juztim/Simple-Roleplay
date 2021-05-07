using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using System.Timers;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public class ShopRobHandler : IScript
    {
        private static Timer ShopRobTimer { get; set; }
        private static Timer CheckRangeTimer { get; set; }
        private static Shopcollection dbItemShop { get; set; }
        private static User player { get; set; }

        [ClientEvent("client:shop:startShopRob")]
        public Task StartShopRob(User x, int shopId)
        {
            player = x;
            dbItemShop = Main.database.ShopCollection.AsQueryable().FirstOrDefault(s => s.shopId == shopId);
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


            NotifyHandler.SendNotification(player, "Du bist dabei den Laden auszurauben!");


            foreach (User p in playerpool)
            {
                if(p.frakId != 1 && p.frakId != 2) continue;
                NotifyHandler.SendNotification(p, $"Ein Laden wird ausgeraubt! ({dbItemShop.shopName})", 10000);
            }
            return Task.CompletedTask;
        }

        private void CheckRange(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Check range Timer!");
            if (!player.Position.IsInRange(dbItemShop.Pos.ToAltPos(), 7f))
            {
                NotifyHandler.SendNotification(player,
                    "Du hast dich zu weit entfernt! Der Ladenraub wurde abgebrochen!");
                var playerpool = Alt.GetAllPlayers();
                foreach (User p in playerpool)
                {
                    if (p.frakId != 1 && p.frakId != 2) continue;
                    NotifyHandler.SendNotification(p, $"Ein Ladenraub wurde vorzeitig abgebrochen! ({dbItemShop.shopName})", 10000);
                }
                Utils.Serverglobals.IsShopRobActive = false;
                ShopRobTimer.Dispose();
                CheckRangeTimer.Dispose();
            }

        }

        private void ShopRobFinished(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Shoprob finished!");
            NotifyHandler.SendNotification(player, "Ladenraub erfolgreich!");
            Utils.Serverglobals.IsShopRobActive = false;
            ShopRobTimer.Dispose();
            CheckRangeTimer.Dispose();

        }
    }
}
