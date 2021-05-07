using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{
    public class PdAppHandler : IScript
    {
        [ClientEvent("server:pd:findNames")]
        public Task pdApp_Search(User player, string wantedName)
        {
            var tempname = wantedName.Split(" ");
            var foundChars = new List<Characters>();
            if (tempname.Length == 2)
            {
                var name = tempname[0] + tempname[1];
                var results = Main.database.CharacterCollection.AsQueryable().Where(c => c.isOnline).ToList();


                foreach (var r in results)
                {
                    var resultName = r.firstName += r.lastName;
                    if (!resultName.ToLower().Contains(name.ToLower())) continue;
                    foundChars.Add(r);
                }
            }
            else if(tempname.Length == 1)
            {
                var results = Main.database.CharacterCollection.AsQueryable()
                    .Where(c => c.isOnline)
                    .Where(c => c.firstName.ToLower().Contains(tempname[0].ToLower()) || c.lastName.ToLower().Contains(tempname[0].ToLower()))
                    .ToList();

                foreach (var r in results)
                {
                    foundChars.Add(r);
                }
                
            }
            
            player.Emit("client:pdapp:fillNames", JsonConvert.SerializeObject(foundChars));
            
            /*foreach (var c in results)
            {
                Console.WriteLine($"{c.firstName} {c.lastName}");   
            }*/
            return Task.CompletedTask;
        }

        [ClientEvent("server:pdApp:openFile")]
        public async Task openPdFile(User player, string playerId)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(int.Parse(playerId));
            var dbCharName = dbChar.firstName + " " + dbChar.lastName;
            var akten = new List<Akte>();

            foreach (var akte in dbChar.Akten)
            {
                var dbAkte = Main.database.FileCollection.AsQueryable().FirstOrDefault(x => x.Akten.Any(y => y.AktenId == akte)).Akten.FirstOrDefault(a => a.AktenId == akte);

                var newAkte = new Akte() { AktenId = dbAkte.AktenId, Name = dbAkte.Name, Fine = dbAkte.Fine, JailTime = dbAkte.JailTime };
                akten.Add(newAkte);


            }

           

            player.Emit("client:pdApp:openRequestedFile", dbCharName, JsonConvert.SerializeObject(akten));

        }

        [ClientEvent("server:pdApp:requestAvailableFiles")]
        public async Task getAvailableFiles(User player)
        {
            var akten = Main.database.FileCollection.AsQueryable()
                .OrderBy(c => c.CategoryId)
                .ToList();
            var aktenHTML = "";

            foreach (AktenCollection akte in akten)
            {
                var listHTML = "";
                foreach (var file in akte.Akten)
                {
                    listHTML += $"<li><a href='#' data-aktenid='{file.AktenId}'>{file.Name}</a></li>";
                }
                aktenHTML += $"<li><div class='link'>{akte.CategoryName}<i class='fa fa-chevron-down'></i></div><ul class='submenu'>{listHTML}</ul></li>";
            }

            player.Emit("client:pdApp:giveAvailableFiles", aktenHTML);
        }

        [ClientEvent("server:pdApp:saveFiles")]
        public async Task savePdFiles(User player, string filesJSON, string targetPlayerId)
        {
            var targetChar = Utils.Utils.GetDatabaseCharacter(int.Parse(targetPlayerId));
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            NotifyHandler.sendPdNotify($"{dbChar.firstName} {dbChar.lastName} hat die Akte von {targetChar.firstName} {targetChar.lastName} erweitert!", 3000);
            var files = JsonConvert.DeserializeObject<List<int>>(filesJSON);

            foreach (var fileID in files)
            {
                targetChar.Akten.Add(fileID);
            }
            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == targetChar.playerId, targetChar);

        }
        
        [ClientEvent("server:pdApp:deleteFiles")]
        public async Task deletePdFiles(User player, string filesJSON, string targetPlayerId)
        {
            var targetChar = Utils.Utils.GetDatabaseCharacter(int.Parse(targetPlayerId));
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            
            var files = JsonConvert.DeserializeObject<List<int>>(filesJSON);
            foreach (var fileID in files)
            {
                if(!targetChar.Akten.Contains(fileID)) break;
                    targetChar.Akten.Remove(fileID);
            }
            
            NotifyHandler.sendPdNotify($"{dbChar.firstName} {dbChar.lastName} hat Akten von {targetChar.firstName} {targetChar.lastName} gelöscht!", 3000);
            Main.database.CharacterCollection.ReplaceOne(c => c.playerId == targetChar.playerId, targetChar);

        }
    }
}
