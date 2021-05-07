using AltV.Net;
using MongoDB.Driver;
using Simple_Roleplay.Factories;
using System.Linq;

namespace Simple_Roleplay.Handlers
{
    public class ATMHandler : IScript
    {
        [ClientEvent("client:atm:einzahlen")]
        public void AtmEinzahlen(User player, string money)
        {
            var moneyCount = int.Parse(money);

            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
            var konto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.objectOwnerId == dbChar.id);
            var tempKonto = konto;

            var tempChar = dbChar;
            tempChar.moneyHand -= moneyCount;

            tempKonto.kontoStand += moneyCount;
            Main.database.BankCollection.ReplaceOne(k => k.objectOwnerId == dbChar.id, tempKonto);
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == player.uid, tempChar);
        }

        [ClientEvent("client:atm:auszahlen")]
        public void AtmAuszahlen(User player, string money)
        {
            var moneyCount = int.Parse(money);

            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
            var konto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.objectOwnerId == dbChar.id);
            var tempKonto = konto;

            var tempChar = dbChar;
            tempChar.moneyHand += moneyCount;

            tempKonto.kontoStand -= moneyCount;

            Main.database.BankCollection.ReplaceOne(k => k.objectOwnerId == dbChar.id, tempKonto);
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == player.uid, tempChar);
        }

        [ClientEvent("client:atm:überweisen")]
        public void AtmAuszahlen(User player, string konto, string money)
        {
            var moneyCount = int.Parse(money);
            var kontonummer = int.Parse(konto);

            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
            var localKonto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.objectOwnerId == dbChar.id);
            var receiveKonto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.kontoNummer == kontonummer);

            var tempLocalKonto = localKonto;
            var tempReceiveKonto = receiveKonto;

            tempLocalKonto.kontoStand -= moneyCount;
            tempReceiveKonto.kontoStand += moneyCount;

            Main.database.BankCollection.ReplaceOne(k => k.objectOwnerId == dbChar.id, tempLocalKonto);
            Main.database.BankCollection.ReplaceOne(k => k.kontoNummer == kontonummer, tempReceiveKonto);

        }
    }
}
