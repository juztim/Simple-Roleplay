using AltV.Net;
using AltV.Net.Elements.Entities;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Linq;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Handlers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Simple_Roleplay.Factories
{
    public class User : Player
    {
        public ObjectId uid { get; set; }
        public string Username { get; set; }
        public int adminLevel { get; set; }
        public bool isAduty { get; set; }
        public long sex { get; set; }
        public int playerId { get; set; }
        public int paintballArena { get; set; }
        public int frakId { get; set; }
        public int IsCalling { get; set; }
        public bool IsJobDuty { get; set; }
        public int CurrentAmmo { get; set; }
        public User(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {
            
        }

#pragma warning disable 1998
        internal async Task UpdateMoneyHud(int money)
#pragma warning restore 1998
        {
            this.Emit("client:hud:updatemoney", money);
        }

        internal async Task<bool> removeMoneyAsync(int price)
        {
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == this.uid);
            var oldMoney = dbChar.moneyHand;
            dbChar.moneyHand -= price;
            if(dbChar.moneyHand < 0)
            {
                dbChar.moneyHand = oldMoney;
                await NotifyHandler.SendNotification(this, "Du hast nicht genug Geld!");
                return false;
            }
            
            await Main.database.CharacterCollection.ReplaceOneAsync(c => c.ownerObjId == this.uid, dbChar);
            await this.UpdateMoneyHud(dbChar.moneyHand);

            return true;
        }

        internal async Task<bool> addItem(int id, int amount)
        {
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == this.uid);
            var dbInv = dbChar.Inventar;
            var Item = new Item() { itemId = id, amount = amount };
            var dbInvItem = dbInv.FirstOrDefault(i => i.itemId == id);
            if(dbInvItem != null)
            {
                dbInvItem.amount += amount;
            }
            else
            {
                Console.WriteLine("ADD: " + id + " " + amount);
                dbInv.Add(Item);
            }
            double invWeight = 0;
            foreach (var item in dbInv)
            {
                var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == item.itemId);
                invWeight += dbItem.weight * item.amount;
            }
            if(invWeight >= 50)
            {
                await NotifyHandler.SendNotification(this, "Dein Inventar ist voll!", 3000);
                return false;
            }
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == this.uid, dbChar);
            return true;
        }

        internal async Task<bool> removeItem(int itemId, int amount)
        {
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == this.uid);
            var dbInv = dbChar.Inventar;
            var dbInvItem = dbInv.FirstOrDefault(i => i.itemId == itemId);
            if (dbInvItem != null)
            {
                dbInvItem.amount -= amount;
                if(dbInvItem.amount == 0)
                {
                    dbChar.Inventar.Remove(dbInvItem);
                }

                if (dbInvItem.amount < 0) return false;
            }
            else
            {
                return false;
            }
            await Main.database.CharacterCollection.ReplaceOneAsync(c => c.ownerObjId == this.uid, dbChar);
            return true;
        }

        internal bool HasItem(int itemId)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(this);
            var existsItem = dbChar.Inventar.FirstOrDefault(i => i.itemId == itemId);
            return existsItem != null;
        }

        /// <summary>
        /// Check If Player has Item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        internal bool HasItem(int itemId, int amount)
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(this);
            var existsItem = dbChar.Inventar.FirstOrDefault(i => i.itemId == itemId);
            if (existsItem == null) return false;
            if (existsItem.amount < amount) return false;
            return true;
        }

        internal Task SaveWeapons()
        {
            var dbChar = Utils.Utils.GetDatabaseCharacter(this);
            Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == this.uid, dbChar);
            return Task.CompletedTask;
        }

        
    }

    public class UserFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(IntPtr playerPointer, ushort id)
        {
            return new User(playerPointer, id);
        }
    }
}
