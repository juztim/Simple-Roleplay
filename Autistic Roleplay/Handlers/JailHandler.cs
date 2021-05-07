using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using AltV.Net;
using AltV.Net.Data;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public static class JailHandler
    {
        private static Timer _JailTimer { get; set; }

        public static async Task initJailTimer()
        {
            Console.WriteLine("Initialized JailTimer");
            _JailTimer = new Timer(1000) {Interval = 1000, Enabled = true};
            _JailTimer.Elapsed += HandleJailTick;
        }

        private static void HandleJailTick(Object source, ElapsedEventArgs e)
        {
            if (e == null) throw new ArgumentNullException(nameof(e));
            foreach (var c in Main.database.CharacterCollection.AsQueryable().Where(ch => ch.isOnline && ch.jailTime > 0))
            {
                c.jailTime -= 1;
                Main.database.CharacterCollection.ReplaceOne(ch => ch.playerId == c.playerId, c);
                Console.WriteLine("Removed 1 from " + c.firstName + " " + c.lastName + ". Remaining: " + c.jailTime);

                if (c.jailTime < 1)
                {
                    HandleFreeJail(c);
                }
            }
        }

        private static void HandleFreeJail(Characters c)
        {
            if(c == null) throw new ArgumentNullException(nameof(c));
            var playerPool = Alt.GetAllPlayers();
            foreach (User player in playerPool)
            {
                if (player.playerId != c.playerId) continue;
                if (player.Position.IsInRange(new Position(1729.25f, 2563.642578125f, 45.56489944458008f), 70f))
                {
                    player.Position = new Position(1847.76171875f, 2585.59228515625f, 45.67206954956055f);
                }

                NotifyHandler.SendNotification(player, "Du bist nun aus dem Gefängnis entlassen!");
            }
        }
    }
}