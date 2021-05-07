using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;
using System.Timers;
using MongoDB.Bson;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Handlers
{
    public class ItemProducerHandler : IScript
    {
        public static Timer ProducerTimer { get; set; }

        [ClientEvent("server:producer:fill")]
        public async Task Producer_Fill(User player, string producerId)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var dbProducer = Main.database.ItemProductionCollection.AsQueryable().FirstOrDefault(p => p.Id == int.Parse(producerId));
            if (int.Parse(producerId) != dbChar.ItemVerarbeiter.VerarbeiterId && dbChar.ItemVerarbeiter.VerarbeiterId != 0)
            {
                NotifyHandler.SendNotification(player, "Du verarbeitest bereits an einem anderen Ort!");
            }
            var limit = 1000 - dbChar.ItemVerarbeiter.CurrentIn;
            var addLimit = (limit / dbProducer.NeededItemAmount) * dbProducer.NeededItemAmount;
            if (!await player.removeItem(dbProducer.NeededItemId, addLimit))
            {
                dbChar.ItemVerarbeiter.CurrentIn += addLimit;
                dbChar.ItemVerarbeiter.VerarbeiterId = dbProducer.Id;
                await NotifyHandler.SendNotification(player, $"Du hast erfolgreich {addLimit} Items hineingelegt!");
                await Main.database.CharacterCollection.ReplaceOneAsync(c => c.playerId == player.playerId, dbChar);
                return;
            }
            try
            {
                Console.WriteLine($"NeededItem: " + Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == dbProducer.NeededItemId).name);
                if (!player.HasItem(dbProducer.NeededItemId, dbProducer.NeededItemAmount))
                {
                    NotifyHandler.SendNotification(player, "Dir fehlen Items zum Verarbeiten!");
                    return;
                }
                var playerMax = dbChar.Inventar.FirstOrDefault(i => i.itemId == dbProducer.NeededItemId).amount;
                var playerLimit = (playerMax / dbProducer.NeededItemAmount) * dbProducer.NeededItemAmount;
                if(playerMax == 0) return;
                if (!await player.removeItem(dbProducer.NeededItemId, playerLimit))
                {
                    NotifyHandler.SendNotification(player, "Fehler beim Auffüllen! [ITMP-001]");
                    return;
                }
                dbChar.ItemVerarbeiter.CurrentIn += playerLimit;
                dbChar.ItemVerarbeiter.VerarbeiterId = dbProducer.Id;
                NotifyHandler.SendNotification(player, $"Du hast erfolgreich {playerLimit} Items hineingelegt!");
                Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbChar);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [ClientEvent("server:producer:empty")]
        public async Task Producer_Empty(User player, string producerId)
        {
            var dbProducer = Main.database.ItemProductionCollection.AsQueryable().FirstOrDefault(ip => ip.Id == int.Parse(producerId));
            await player.addItem(dbProducer.OutComeItemId, Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.playerId == player.playerId).ItemVerarbeiter.CurrentOut);
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            await NotifyHandler.SendNotification(player, $"Du hast {dbChar.ItemVerarbeiter.CurrentOut} Items herausgenommen!");
            dbChar.ItemVerarbeiter.CurrentOut = 0;
            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbChar);
            return;
        }

        public static async Task Init()
        {
            ProducerTimer = new Timer();
            ProducerTimer.Interval = 5000;
            ProducerTimer.Enabled = true;
            ProducerTimer.Elapsed += ProducerTimer_Elapsed;
        }

        private static void ProducerTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (User player in Alt.GetAllPlayers())
            {
                
                var dbChar = Utils.Utils.GetDatabaseCharacter(player);
                if (dbChar == null || dbChar.ItemVerarbeiter.VerarbeiterId == 0) return;
                var dbVerarbeiter = Main.database.ItemProductionCollection.AsQueryable().FirstOrDefault(ip => ip.Id == dbChar.ItemVerarbeiter.VerarbeiterId);
                if(dbVerarbeiter == null) return;
                dbChar.ItemVerarbeiter.CurrentIn -= dbVerarbeiter.NeededItemAmount;
                dbChar.ItemVerarbeiter.CurrentOut += dbVerarbeiter.OutComeAmount;
                if (dbChar.ItemVerarbeiter.CurrentIn == 0)
                {
                    if(dbChar.ItemVerarbeiter.VerarbeiterId == 0) return;
                    dbChar.ItemVerarbeiter.VerarbeiterId = 0;
                    Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbChar);
                    NotifyHandler.SendNotification(player, "Der Verarbeitungsprozess ist beendet!", 30000);
                    return;
                }
                if (dbChar.ItemVerarbeiter.CurrentIn < 0 || dbChar.ItemVerarbeiter.CurrentOut >= 1000) return;
                Main.database.CharacterCollection.ReplaceOne(c => c.playerId == player.playerId, dbChar);
                Main.database.ItemProductionCollection.ReplaceOne(ip => ip.Id == dbChar.ItemVerarbeiter.VerarbeiterId, dbVerarbeiter);
            }
        }
    }
}
