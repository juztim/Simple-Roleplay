using System;
using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Threading.Tasks;
using System.Linq;
using Simple_Roleplay.Utils;

namespace Simple_Roleplay.Handlers
{
    public class PlayerDisconnectHandler : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerDisconnect)]
        public Task onPlayerDisconnect(User player, string reason)
        {
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
            if (dbChar == null)
            {
                return Task.CompletedTask;
            }


            foreach (User p in Alt.GetAllPlayers().Where(x => player.Position.IsInRange(x.Position, 20f)))
            {
                NotifyHandler.SendNotification(p, $"Anti-Offlineflucht {dbChar.firstName} {dbChar.lastName} ist disconnected! Grund: {reason}");
                Console.WriteLine("Disconnect Reason: " + reason);
            }

            

            //save position
            if(player.paintballArena == 0)
            {
                dbChar.pos_x = player.Position.X;
                dbChar.pos_y = player.Position.Y;
                dbChar.pos_z = player.Position.Z;
                dbChar.health = player.Health;
                dbChar.armor = player.Armor;
                dbChar.isOnline = false;
            }
            if(player.paintballArena != 0)
            {
                PaintballHandler.HandleDisconnect(player);
            }

            Main.database.CharacterCollection.ReplaceOne(x => x.ownerObjId == player.uid, dbChar);
            return Task.CompletedTask;
        }
    }
}
