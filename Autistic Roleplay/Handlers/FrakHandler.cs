using AltV.Net;
using AltV.Net.Resources.Chat.Api;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System;
using System.Threading.Tasks;
using System.Linq;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Handlers
{
    public class FrakHandler : IScript
    {
        [Command("invite")]
        public async Task invite_CMD(User player, string str_playerId)
        {
            int playerId = int.Parse(str_playerId);
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.playerId == playerId);
            var dbFrak = Main.database.FractionCollection.AsQueryable().FirstOrDefault(f => f.frakId == player.frakId);
            if (dbFrak == null) return;
            if (dbFrak.frakMember.FirstOrDefault(f => f.playerid == player.playerId).rank < 11)
            {
                await NotifyHandler.SendNotification(player, "Du bist nicht berechtigt Leute einzuladen!");
                return;
            }
            if(dbChar == null)
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler existiert nicht!");
                return;
            }
            if(!dbChar.isOnline)
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler ist nicht online!");
                return;
            }
            if(dbChar.frakId != null)
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler ist bereits in einer Fraktion!");
                return;
            }
            foreach (User p in Alt.GetAllPlayers())
            {
                if(p.playerId == playerId)
                {
                    await NotifyHandler.SendNotification(player, $"Du hast {dbChar.firstName} {dbChar.lastName} ({dbChar.playerId}) zu {dbFrak.frakName} eingeladen!");
                    dbChar.frakId = dbFrak.frakId;
                    p.frakId = dbFrak.frakId;
                    dbFrak.frakMember.Add(new FrakMember() { playerid = playerId, joinDate = DateTime.Now.ToLocalTime(), rank = 0 });
                    break;
                }
                
            }
            
            await Main.database.CharacterCollection.ReplaceOneAsync(c => c.playerId == playerId, dbChar);
            await Main.database.FractionCollection.ReplaceOneAsync(f => f.frakId == player.frakId, dbFrak);
        }

        [Command("uninvite")]
        public async Task uninvite_CMD(User player, string str_playerId)
        {
            int playerId = int.Parse(str_playerId);
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.playerId == playerId);
            var dbFrak = Main.database.FractionCollection.AsQueryable().FirstOrDefault(f => f.frakId == player.frakId);
            if (dbFrak == null) return;
            if (dbFrak.frakMember.FirstOrDefault(f => f.playerid == player.playerId).rank < 11)
            {
                await NotifyHandler.SendNotification(player, "Du bist nicht berechtigt Leute rauszuwerfen!");
                return;
            }
            if (dbChar == null)
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler existiert nicht!");
                return;
            }
            if (dbChar.frakId != dbFrak.frakId)
            {
                await NotifyHandler.SendNotification(player, "Dieser Spieler ist kein Mitglied dieser Fraktion!");
                return;
            }
            dbChar.frakId = null;

            foreach (var member in dbFrak.frakMember)
            {
                if(member.playerid == playerId)
                {
                    dbFrak.frakMember.Remove(member);
                    await NotifyHandler.SendNotification(player, $"Du hast {dbChar.firstName} {dbChar.lastName} ({dbChar.playerId}) aus {dbFrak.frakName} geworfen!");
                    break;
                }
            }

            await Main.database.CharacterCollection.ReplaceOneAsync(c => c.playerId == playerId, dbChar);
            await Main.database.FractionCollection.ReplaceOneAsync(f => f.frakId == player.frakId, dbFrak);
        }
    }
}
