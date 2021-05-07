using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;

namespace Simple_Roleplay.Handlers
{
    public class BuergerbueroHandler : IScript
    {
        [ClientEvent("server:buergerbuero:senddata")]
        public void receiveBuergerBueroData_Handler(User player, dynamic vorname, dynamic nachname, dynamic geburtsdatum, dynamic geburtsort)
        {
            Alt.Log("server:buergerbuero:senddata called!");
            Alt.Log($"{vorname} {nachname} {geburtsdatum} {geburtsort}");
            var dbchar = Utils.Utils.GetDatabaseCharacter(player);
            var tempchar = dbchar;
            dbchar.firstName = vorname;
            dbchar.lastName = nachname;
            dbchar.birthDay = geburtsdatum;
            dbchar.birthplace = geburtsort;
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == player.uid, tempchar);
        }
    }
}
