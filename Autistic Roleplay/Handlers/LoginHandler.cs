using System;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Simple_Roleplay.Database.Collections;
using Simple_Roleplay.Factories;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AltV.Net.Resources.Chat.Api;
using Simple_Roleplay.Interactions.Types;
using System.Collections.Generic;

namespace Simple_Roleplay.Handlers
{
    class LoginHandler : IScript
    {
        #region Login
        [ClientEvent("client:login:login")]
        public async Task ValidateLoginAsync(User player, string username, string password)
        {

            Users dbUser = Main.database.UserCollection.AsQueryable().FirstOrDefault(x => x.Username == username);

            if(dbUser == null) return;
            if (!BCrypt.Net.BCrypt.Verify(password, dbUser.Password))
            {
                player.Emit("client:login:modal", "Fehler", "Falsches Passwort!");
                return;
            }
            Characters dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == dbUser.id);
            var tempChar = dbChar;
            if (dbChar == null)
            {
                return;
            }

            if (!dbUser.IsWhitelisted)
            {
                player.Emit("client:login:modal", "Fehler", "Dieser Account ist noch nicht gewhitelisted!");
                return;
            }
            player.Username = dbUser.Username;
            player.Dimension = 0;
            player.adminLevel = dbUser.adminLevel;
            player.Model = (uint)PedModel.FreemodeMale01;
            if (dbChar.charData.sex != null) player.sex = dbChar.charData.sex.Value;
            player.playerId = dbChar.playerId;
            if(dbChar.frakId.HasValue) player.frakId = dbChar.frakId.Value;
            player.Emit("server:login:finished");


            //an alter pos spawnen
            if (dbChar.pos_x.HasValue)
            {
                var chardata = JsonConvert.SerializeObject(dbChar.charData);
                player.Spawn(new Position(dbChar.pos_x.Value, dbChar.pos_y.Value, (dbChar.pos_z.Value + 0.1f)),0);
                player.Emit("character:Sync", chardata, true);
            }
            else
            {
                 player.Spawn(new Position(-1045.6171875f, -2750.943115234375f, 21.36341667175293f), 0);
                 player.Rotation = new Rotation(0, 0, 326.61578369140625f);
                
            }
            if(dbChar.Clothes == null)
            {
                var Clothes = new Clothing()
                {
                    Mask = new Component() { drawableId = 0, colorId = 0 },
                    Torso = new Component() { drawableId = 0, colorId = 0 },
                    Legs = new Component() { drawableId = 0, colorId = 0 },
                    Bag = new Component() { drawableId = 0, colorId = 0 },
                    Shoes = new Component() { drawableId = 0, colorId = 0 },
                    Accessoires = new Component() { drawableId = 0, colorId = 0 },
                    Undershirt = new Component() { drawableId = 0, colorId = 0 },
                    Armor = new Component() { drawableId = 0, colorId = 0 },
                    Decals = new Component() { drawableId = 0, colorId = 0 },
                    Tops = new Component() { drawableId = 0, colorId = 0 },
                    Hats = new Component() { drawableId = 0, colorId = 0 },
                    Glasses = new Component() { drawableId = 0, colorId = 0 },
                    Ears = new Component() { drawableId = 0, colorId = 0 },
                    Watches = new Component() { drawableId = 0, colorId = 0 },
                    Bracelets = new Component() { drawableId = 0, colorId = 0 },

                };
                tempChar.Clothes = Clothes;
            }
            if(dbChar.Inventar == null)
            {
                tempChar.Inventar = new List<Item> {new Item {amount = 39, itemId = 1}};
            }


            player.Emit("client:hud:displayNotify", "Erfolgreich eingeloggt!");
            player.Emit("SaltyChat_OnConnected");
            Alt.Emit("PlayerLoggedIn", player);

            player.uid = dbUser.id;
            player.Health = dbChar.health;
            player.Armor = dbChar.armor;
            player.paintballArena = 0;

            var tempUser = dbUser;
            tempUser.lastLogin = System.DateTime.Now.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            tempChar.isOnline = true;


            await Main.database.CharacterCollection.ReplaceOneAsync(c => c.ownerObjId == player.uid, tempChar);
            await Main.database.UserCollection.ReplaceOneAsync(x => x.Username == username, tempUser);
            var itemshopList = JsonConvert.SerializeObject(itemShopInteraction.shopList);
            var garageList = JsonConvert.SerializeObject(GarageInteractions.garageList);
            var tankstellenList = JsonConvert.SerializeObject(fuelStationInteract.fuelStationList);
            var vehshopList = JsonConvert.SerializeObject(VehShopInteraction.vehShops);

            var discountStoreList = JsonConvert.SerializeObject(ClothingStoreInteraction.clothingStores.Where(c => c.storeType == 3));
            var suburbanList = JsonConvert.SerializeObject(ClothingStoreInteraction.clothingStores.Where(c => c.storeType == 2));
            var ponsonlist =
                JsonConvert.SerializeObject(ClothingStoreInteraction.clothingStores.Where(c => c.storeType == 1));
            
           var frakgaragelist = JsonConvert.SerializeObject(FrakGarageInteraction.frakGarages);
           var frakNpcList = JsonConvert.SerializeObject(FrakNPCInteraction.frakNPCList);
           var ammunationList = JsonConvert.SerializeObject(AmmunationInteraction.AmmunationList);

            player.Emit("client:interactions:loadGarage", garageList);
            player.Emit("client:interactions:loadShops", itemshopList);
            player.Emit("client:interactions:loadFuelstations", tankstellenList);
            player.Emit("client:interactions:loadClothingStores", discountStoreList, suburbanList, ponsonlist); 
            player.Emit("client:interactions:loadCarDealers", vehshopList);
            player.Emit("client:interactions:loadFrakGarages", frakgaragelist);
            player.Emit("client:interactions:loadFrakNPCs", frakNpcList);
            player.Emit("client:interactions:loadAmmunations", ammunationList);

            player.Emit("server:createHud", dbChar.moneyHand);

            await Utils.Utils.LoadClothes(player, dbChar);

            player.SetStreamSyncedMetaData("sharedUsername", dbChar.firstName + " " + dbChar.lastName);
            player.SetSyncedMetaData("ADMINLEVEL", dbUser.adminLevel);
            player.SetStreamSyncedMetaData("sharedId", dbChar.playerId);
            player.IsJobDuty = false;


        }
        #endregion
        #region Register
        [ClientEvent("client:login:register")]
        public Task RegisterUser(User player, string username, string password)
        {
            Console.WriteLine("151");
            //Check if user exists
            var dbUser = Main.database.UserCollection.AsQueryable().FirstOrDefault(x => x.Username.ToLower() == username.ToLower());
            if (dbUser != null)
            {
                player.Emit("client:login:modal", "Fehler", "Dieser Benutzername ist bereits vergeben!");
                return Task.CompletedTask;
            }
            //hash pw
            var hashedPw = BCrypt.Net.BCrypt.HashString(password);
            Console.WriteLine("161");
            //create user
            var newUser = new Users() { Username = username, Password = hashedPw};

            //emit client success
            player.Emit("server:register:finished");

            //add to db
            Main.database.UserCollection.InsertOne(newUser);
            Console.WriteLine("170");
            var Clothes = new Clothing()
            {
                Mask = new Component() { drawableId = 0, colorId = 0 },
                Torso = new Component() { drawableId = 0, colorId = 0 },
                Legs = new Component() { drawableId = 0, colorId = 0 },
                Bag = new Component() { drawableId = 0, colorId = 0 },
                Shoes = new Component() { drawableId = 0, colorId = 0 },
                Accessoires = new Component() { drawableId = 0, colorId = 0 },
                Undershirt = new Component() { drawableId = 0, colorId = 0 },
                Armor = new Component() { drawableId = 0, colorId = 0 },
                Decals = new Component() { drawableId = 0, colorId = 0 },
                Tops = new Component() { drawableId = 0, colorId = 0 },
                Hats = new Component() { drawableId = 0, colorId = 0 },
                Glasses = new Component() { drawableId = 0, colorId = 0 },
                Ears = new Component() { drawableId = 0, colorId = 0 },
                Watches = new Component() { drawableId = 0, colorId = 0 },
                Bracelets = new Component() { drawableId = 0, colorId = 0 },

            };
            Console.WriteLine("190");
            var charList = Main.database.CharacterCollection.AsQueryable().ToList();
            int nextPlayerId;
            if (charList.Count == 0) nextPlayerId = 1;
            else nextPlayerId = charList.Last().playerId + 1;



                bool numberNotExisting = false;
            int nummer = 0;
            Console.WriteLine("197");
            do
            {
                var rand = new Random();
                nummer = rand.Next(10000, 99999);

                var nummerExists = Main.database.CharacterCollection.AsQueryable().Where(c => c.Number == nummer);
                if (nummerExists != null)
                {
                    numberNotExisting = true;
                }

            } while (!numberNotExisting);
            
            Console.WriteLine("211");
            //create new char
            var newChar = new Characters() { ownerObjId = newUser.id, moneyHand = 5000, Clothes = Clothes, playerId = nextPlayerId, Inventar = new List<Item>(), Number = nummer};

            //create new Bankacc
            var nextKontoId = Main.database.BankCollection.AsQueryable().ToList().Count + 1;
            Console.WriteLine("217");
            var newBankAcc = new KontoCollection() {kontoNummer = nextKontoId, kontoPin = 0000, kontoStand = 0, objectOwnerId = newChar.id};
            Main.database.BankCollection.InsertOne(newBankAcc);
            Console.WriteLine("219");
            //add to db
            Main.database.CharacterCollection.InsertOne(newChar);
            player.playerId = newChar.playerId;
            player.uid = newUser.id;


            Console.WriteLine("227");
            player.Spawn(new Position(-812.0f, 175.0f, 76f), 0);
            player.Rotation = new Rotation(0, 0, 102.1003189086914f);
            //start charcreator
            player.Emit("character:Edit");
            player.Dimension = player.Id;
            




            return Task.CompletedTask;
        }
        #endregion
        #region Charakter Fertig
        [ClientEvent("character:Done")]
        public void CharacterDone(User player, string data)
        {

            var json = JsonConvert.DeserializeObject<Chardata>(data);


            Chardata charData = new Chardata()
            {
                sex = json.sex,
                faceFather = json.faceFather,
                faceMother = json.faceMother,
                skinFather = json.skinFather,
                faceMix = json.faceMix,
                skinMix = json.skinMix,
                structure = json.structure,
                hair = json.hair,
                hairColor1 = json.hairColor1,
                hairColor2 = json.hairColor2,
                hairOverlay = json.hairOverlay,
                facialHair = json.facialHair,
                facialHairColor1 = json.facialHairColor1,
                facialHairOpacity = json.facialHairOpacity,
                eyebrows = json.eyebrows,
                eyebrowsOpacity = json.eyebrowsOpacity,
                eyes = json.eyes,
                opacityOverlays = json.opacityOverlays,
                colorOverlays = json.colorOverlays
            };
            player.Kick("Bitte im TS einfinden für das Whitelistgespräch -> simple-roleplay.de");
            //var dbUser = Utils.Utils.GetDatabasePlayer(player);
            var dbChar = Main.database.CharacterCollection.AsQueryable().FirstOrDefault(c => c.ownerObjId == player.uid);
            if (dbChar != null)
            {
                Console.WriteLine("DBCHAR NOT NULL");
                var localChar = dbChar;
                localChar.charData = charData;
                localChar.pos_x = -1045.6171875f;
                localChar.pos_y = -2750.943115234375f;
                localChar.pos_z = 21.36341667175293f;

                Main.database.CharacterCollection.ReplaceOne(c => c.ownerObjId == player.uid, localChar);
            }


            
            
            

            player.Dimension = 0;


        }
        #endregion
    }
}
