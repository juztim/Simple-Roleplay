using AltV.Net;
using AltV.Net.Data;
using Simple_Roleplay.Interactions.Types;
using Simple_Roleplay.Utils;
using Simple_Roleplay.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AltV.Net.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using Simple_Roleplay.Database.Collections;
using Newtonsoft.Json;
using Simple_Roleplay.Handlers;

namespace Simple_Roleplay.Interactions
{
    public class interactionHandler : IScript
    {
        [ClientEvent("server:pressE")]
        public async Task ClientStartInteract(User player)
        {
            
            var dbChar = Utils.Utils.GetDatabaseCharacter(player);
            var buergerBuero = BuergerBueros.buergerBueroList.ToList().FirstOrDefault(x => player.Position.IsInRange(x.pos, 1.5f));
            var vehShop = ServerNPCS.ServerNPCList.ToList().FirstOrDefault(x => player.Position.IsInRange(x.pos, 1.5f));
            var atm = ATMS.atmInteractions.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Position), 1.5f));
            var garage = GarageInteractions.garageList.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 1.5f));
            var itemshop = itemShopInteraction.shopList.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 2f));
            var paintball = paintballInteraction.paintballNPCs.ToList().FirstOrDefault(x => player.Position.IsInRange(x.pos, 2f));
            var tankstelle = fuelStationInteract.fuelStationList.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 20f));
            var kleidershop = ClothingStoreInteraction.clothingStores.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 5f));
            var feld = feldInteraction.feldListe.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.pos), x.radius));
            var fraknpc = FrakNPCInteraction.frakNPCList.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 3f));
            var carshop = VehShopInteraction.vehShops.ToList().FirstOrDefault(x => player.Position.IsInRange(ATMS.atmInteraction.toAltPos(x.Pos), 2f));
            var frakGarage = FrakGarageInteraction.frakGarages.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Pos.ToAltPos(), 2.5f));
            var ammunation = AmmunationInteraction.AmmunationList.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Pos.ToAltPos(), 2.5f));
            var kleiderschrank = KleiderSchrankInteraction.KlederSchränke.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Position.ToAltPos(), x.Range));
            var jumppoint = JumpPointInteraction.JumpPointList.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Position.ToAltPos(), 2f));
            var frakkleiderschrank = FrakKleiderSchrankInteraction.KlederSchränke.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Position.ToAltPos(), 2f));
            var itemproducer = ItemProductionInteraction.ItemProductionList.ToList().FirstOrDefault(x => player.Position.IsInRange(x.Position.ToAltPos(), 5f));

            if (buergerBuero != null)
            {
                player.Emit("client:cef:loadBürgerBüro", true);
            }
            if (vehShop != null)
            {
            }
            if (atm != null)
            {
                var konto = Main.database.BankCollection.AsQueryable().FirstOrDefault(k => k.objectOwnerId == dbChar.id);
                player.Emit("client:cef:loadAtm", konto.kontoPin, konto.kontoStand);
            }
            if (garage != null)
            {

                #region Fetch parked in cars
                var carPool = Main.database.CarCollection.AsQueryable().ToList();
                var cars = new List<Carcollection>();
                foreach (var car in carPool)
                {
                    if (car.ownerId == dbChar.playerId && car.parkedIn && car.garageId == garage.garageId || car.allowedIds.Contains(dbChar.playerId) && car.parkedIn && car.garageId == garage.garageId)
                    {
                        var tempCar = new Carcollection()
                        {
                            carmodel = car.carmodel,
                            numPlate = car.numPlate,
                            carId = car.carId,
                            parkedIn = true
                        };
                        cars.Add(tempCar);
                    }
                }
                
                #endregion
                #region Fetch cars nearby
                var carpool = Alt.GetAllVehicles();
                // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
                foreach (Car car in carpool)
                {
                    if(car.frakId > 0) continue;

                    if (car.Position.IsInRange(ATMS.atmInteraction.toAltPos(garage.Pos), 30f))
                    {
                        if (car.frakId != 0) continue;
                        if (car.carId == 0) continue;
                        if (car.ownerId == dbChar.playerId || car.allowedIds.Contains(dbChar.playerId))
                        {
                            var listCar = new Carcollection()
                            {
                                carmodel = car.carmodel,
                                numPlate = car.numPlate,
                                objectId = car.objectId,
                                parkedIn = false

                            };
                            cars.Add(listCar);
                        }
                    }
                    
                }
                var carList = JsonConvert.SerializeObject(cars);
                player.Emit("client:cef:hud:loadgarage", carList, garage.garageId);
                #endregion

            }
            if(itemshop != null)
            {
                var itemList = new List<itemShopInteraction.ShopClientItem>();

                var shopItems = itemshop.shopItems;
                foreach (var item in shopItems)
                {
                  
                    var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == item);
                    var newItem = new itemShopInteraction.ShopClientItem() { name = dbItem.name, price = dbItem.price.Value, itemId = dbItem.itemId };
                    itemList.Add(newItem);
                    
                }
                player.Emit("client:cef:loadItemShop", JsonConvert.SerializeObject(itemList) ,itemshop.shopId);
            }
            if(paintball != null)
            {
                var arenaList = Main.database.PaintballCollection.AsQueryable().ToList();
                player.Emit("client:cef:loadPaintball", JsonConvert.SerializeObject(arenaList));
            }

            if(tankstelle != null)
            {
                var vehicle = (Car)player.Vehicle;
                if (player.IsInVehicle)
                {
                    player.Emit("client:hud:showGasstation", vehicle.carId);
                }
            }

            if(kleidershop != null)
            {
               
                List<ClothesCollection> kleiderListe = new List<ClothesCollection>();

                switch (kleidershop.storeType)
                {
                    case 1:
                        kleiderListe = Utils.Utils.Clothes.ponsonList.Where(c => int.Parse(c.gender) == player.sex || int.Parse(c.gender) == 3).ToList();
                        break;
                    case 2:
                        kleiderListe = Utils.Utils.Clothes.suburbanList.Where(c => int.Parse(c.gender) == player.sex || int.Parse(c.gender) == 3).ToList();
                        break;
                    case 3:
                        kleiderListe = Utils.Utils.Clothes.discountList.Where(c => int.Parse(c.gender) == player.sex || int.Parse(c.gender) == 3).ToList();
                        break;
                }

                
                var itemCount = (int)kleiderListe.Count;
                var iterations = Math.Floor((decimal)(itemCount / 200));
                var rest = itemCount % 200;
                for (var i = 0; i < iterations; i++)
                {
                    var skip = i * 200;
                    player.Emit("client:loadClothesStore", JsonConvert.SerializeObject(kleiderListe.Skip(skip).Take(200).ToList()));
                }
                if (rest != 0) player.Emit("client:loadClothesStore", JsonConvert.SerializeObject(kleiderListe.Skip((int)iterations * 200).ToList()));

            }

            if(feld != null)
            {
                player.Emit("client:feld:farmen", feld.id);
            }

            if(fraknpc != null)
            {
                if(player.frakId != fraknpc.frakId)
                {
                    await NotifyHandler.SendNotification(player, $"({fraknpc.frakId}) Keine Rechte!");
                    return;
                }

                var itemList = new List<itemShopInteraction.ShopClientItem>();

                var shopItems = fraknpc.frakShopItems;
                foreach (var item in shopItems)
                {
                    var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == item.itemId);
                    var newItem = new itemShopInteraction.ShopClientItem() { name = dbItem.name, price = item.itemPrice, itemId = item.itemId };
                    itemList.Add(newItem);

                }
                player.Emit("client:loadFrakShop", JsonConvert.SerializeObject(itemList), fraknpc.frakId);     
            }

            if (carshop != null)
            {
                player.Emit("client:loadCarDealer", JsonConvert.SerializeObject(carshop.Cars), carshop.Id);
            }

            if (frakGarage != null)
            {
                if (player.frakId != frakGarage.FrakId)
                {
                    await NotifyHandler.SendNotification(player, $"Keine Rechte [{frakGarage.FrakId}]");
                    return;
                }
                var carPool = Main.database.FrakcarCollection.AsQueryable().ToList();
                var cars = new List<frakCarCollection>();
                foreach (var car in carPool)
                {
                    if (car.frakId == dbChar.frakId && car.parkedIn && car.garageId == frakGarage.Id)
                    {
                        var tempCar = new frakCarCollection()
                        {
                            carmodel = car.carmodel,
                            numPlate = car.numPlate,
                            carId = car.carId,
                            parkedIn = true
                        };
                        cars.Add(tempCar);
                    }
                }
                var carpool = Alt.GetAllVehicles();
                foreach (Car car in carpool)
                {
                    if (car.Position.IsInRange(ATMS.atmInteraction.toAltPos(frakGarage.Pos), 30f))
                    {
                        if (car.frakId != frakGarage.FrakId) continue;
                        if (car.carId == 0) continue;
                        if (car.frakId == dbChar.frakId)
                        {
                            
                            var listCar = new frakCarCollection()
                            {
                                numPlate = car.numPlate,
                                carmodel = car.carmodel,
                                parkedIn = false,
                                carId = car.carId

                            };
                            cars.Add(listCar);
                        }
                    }

                }

                var carList = JsonConvert.SerializeObject(cars);
                
                player.Emit("client:cef:hud:loadfrakgarage", carList, dbChar.frakId);
            }

            if (ammunation != null)
            {
                var itemList = new List<itemShopInteraction.ShopClientItem>();
                foreach (var item in ammunation.Inventory)
                {
                    var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == item.ItemId);
                    var newItem = new itemShopInteraction.ShopClientItem() { name = dbItem.name, price = item.Price, itemId = item.ItemId };
                    itemList.Add(newItem);

                }

                Console.WriteLine($"AmmoID: {ammunation.Id}");
                player.Emit("client:loadAmmunation", JsonConvert.SerializeObject(itemList), ammunation.Id);
            }

            if (kleiderschrank != null)
            {
                //Get Player Clothing

                var clothing = new List<ClientClothing>();

                var ownedClothing = dbChar.OwnedClothes;
                if (player.sex == 1) clothing.Add(new ClientClothing() { ComponentId = 8, ColorId = 0, DrawableId = 57, ClothId = 11586, Name = "Kein Undershirt" });
                if (player.sex == 0) clothing.Add(new ClientClothing() { ComponentId = 8, ColorId = 0, DrawableId = 34, ClothId = 2393, Name = "Kein Undershirt" });


                foreach (var cloth in ownedClothing)
                {
                    

                    var dbCloth = Main.database.ClothesCollection.AsQueryable()
                        .FirstOrDefault(c => c.clothId == cloth.ToString());

                    var newcloth = new ClientClothing() {ComponentId = int.Parse(dbCloth.componentId), ColorId = int.Parse(dbCloth.colorId), DrawableId = int.Parse(dbCloth.drawableId), ClothId = int.Parse(dbCloth.clothId)};
                    clothing.Add(newcloth);

                }

                foreach (var torso in Main.database.ClothesCollection.AsQueryable().Where(c => c.componentId == "3").Where(c => c.gender == player.sex.ToString()))
                {
                    var newcloth = new ClientClothing() { ComponentId = int.Parse(torso.componentId), ColorId = int.Parse(torso.colorId), DrawableId = int.Parse(torso.drawableId), ClothId = int.Parse(torso.clothId) };
                    clothing.Add(newcloth);
                }

                player.Emit("client:openCloset", JsonConvert.SerializeObject(clothing));
            }

            if (jumppoint != null)
            {
                player.Position = jumppoint.TargetPosition.ToAltPos();
            }

            if (frakkleiderschrank != null)
            {
                var clothing = new List<ClientClothing>();
                if (player.sex == 1)
                {
                    clothing.Add(new ClientClothing() { ComponentId = 8, ColorId = 0, DrawableId = 57, ClothId = 11586, Name = "Kein Undershirt"});
                    clothing.Add(new ClientClothing() { PropId = 0, ColorId = 0, DrawableId = 8, ClothId = 15035, Name = "Keine Kopfbedeckung" });
                }

                if (player.sex == 0)
                {
                    clothing.Add(new ClientClothing() { ComponentId = 8, ColorId = 0, DrawableId = 34, ClothId = 2393, Name = "Kein Undershirt" }); 
                    clothing.Add(new ClientClothing() { PropId = 0, ColorId = 0, DrawableId = 57, ClothId = 15036, Name = "Keine Kopfbedeckung" });

                }
                clothing.Add(new ClientClothing() { ComponentId = 7, ColorId = 0, DrawableId = 0, ClothId = 15034, Name = "Kein Accessoir" });
                clothing.Add(new ClientClothing() { ComponentId = 9, ColorId = 0, DrawableId = 0, ClothId = 15030, Name = "Keine Weste" });
                clothing.Add(new ClientClothing() { ComponentId = 1, ColorId = 0, DrawableId = 0, ClothId = 15033, Name = "Keine Maske" });

                var ownedClothing = dbChar.OwnedClothes;


                foreach (var cloth in ownedClothing)
                {

                    var dbCloth = Utils.Utils.CachedColthing
                        .FirstOrDefault(c => c.clothId == cloth.ToString());

                    var newcloth = new ClientClothing() { ComponentId = int.Parse(dbCloth.componentId), ColorId = int.Parse(dbCloth.colorId), DrawableId = int.Parse(dbCloth.drawableId), ClothId = int.Parse(dbCloth.clothId) };
                    clothing.Add(newcloth);

                }

                foreach (var torso in Utils.Utils.CachedColthing.Where(c => c.componentId == "3").Where(c => c.gender == player.sex.ToString()))
                {
                    var newcloth = new ClientClothing() { ComponentId = int.Parse(torso.componentId), ColorId = int.Parse(torso.colorId), DrawableId = int.Parse(torso.drawableId), ClothId = int.Parse(torso.clothId) };
                    clothing.Add(newcloth);
                }

                foreach (var frakClothing in frakkleiderschrank.Clothing)
                {
                    var dbCloth = Utils.Utils.CachedColthing
                        .FirstOrDefault(c => c.clothId == frakClothing.ToString());

                    if(dbCloth.gender.ToLower() != player.sex.ToString().ToLower() && dbCloth.gender.ToLower() != "3") continue;
                    if(string.IsNullOrEmpty(dbCloth.componentId) && string.IsNullOrEmpty(dbCloth.propId)) continue;
                    if (!string.IsNullOrEmpty(dbCloth.componentId))
                    {
                        var newcloth = new ClientClothing() { ComponentId = int.Parse(dbCloth.componentId), ColorId = int.Parse(dbCloth.colorId), DrawableId = int.Parse(dbCloth.drawableId), ClothId = int.Parse(dbCloth.clothId), Name = dbCloth.name };
                        clothing.Add(newcloth);

                    }
                    else if (!string.IsNullOrEmpty(dbCloth.propId))
                    {
                        var newprop = new ClientClothing() { PropId = int.Parse(dbCloth.propId), ColorId = int.Parse(dbCloth.colorId), DrawableId = int.Parse(dbCloth.drawableId), ClothId = int.Parse(dbCloth.clothId), Name = dbCloth.name };
                        clothing.Add(newprop);
                    }

                }
                Console.WriteLine($"OpenCloset {clothing.Count}");
                player.Emit("client:openCloset", JsonConvert.SerializeObject(clothing), player.sex);
            }

            if (itemproducer != null)
            {
                Console.WriteLine("found itemproducer");

                var dbchar = Utils.Utils.GetDatabaseCharacter(player);
                var dbItemInput = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == itemproducer.NeededItemId);
                var dbItemOutput = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == itemproducer.OutComeItemId);
                var producerData = new
                {
                    Name = itemproducer.Name,
                    Id = itemproducer.Id,
                    CurrentIn = dbchar.ItemVerarbeiter.CurrentIn,
                    CurrentOut = dbchar.ItemVerarbeiter.CurrentOut,
                    InputId = itemproducer.NeededItemId,
                    OutputId = itemproducer.OutComeItemId,
                    InputName = dbItemInput.name,
                    OutputName = dbItemOutput.name
                };

                player.Emit("client:openProducer", JsonConvert.SerializeObject(producerData));
            }
        }

        internal static Task LoadInteractions()
        {
            var buergerBuero = new BuergerBueros.BuergerBuero() { id = 0, pos = new Position(105.62637f, -933.0725f, 29.7854f), clientEvent = "buergerBuero" };
            var paintballNPC = new paintballInteraction.paintballNPC() { pos = new Position(214.63145446777344f, -932.1984252929688f, 24.141572952270508f) };

            var atmList = Main.database.AtmCollection.AsQueryable().ToList();
            var garageList = Main.database.GarageCollection.AsQueryable().ToList();
            var itemshopList = Main.database.ShopCollection.AsQueryable().ToList();
            var paintballArenaList = Main.database.PaintballCollection.AsQueryable().ToList();
            var fuelstationList = Main.database.FuelstationCollection.AsQueryable().ToList();
            var clothingstoreList = Main.database.ClothingStoreCollection.AsQueryable().ToList();
            var fieldList = Main.database.FeldCollection.AsQueryable().ToList();
            var frakNPCList = Main.database.FrakNpcCollection.AsQueryable().ToList();
            var carshops = Main.database.VehShopCollection.AsQueryable().ToList();
            var frakgarages = Main.database.FrakGarageCollection.AsQueryable().ToList();
            var ammunations = Main.database.AmmunationCollection.AsQueryable().ToList();
            var kleiderschränke = Main.database.ClosetCollection.AsQueryable().ToList();
            var jumppoints = Main.database.JumpPointCollection.AsQueryable().ToList();
            var frakkleiderschränke = Main.database.FrakClosetCollection.AsQueryable().ToList();
            var itemproducers = Main.database.ItemProductionCollection.AsQueryable().ToList();

            Utils.Utils.CachedColthing = Main.database.ClothesCollection.AsQueryable().ToList();


            Console.WriteLine("------------------------------------------------");
            int count = 0;
            foreach (var atm in atmList)
            {
                count ++;
                var newAtm = new ATMS.atmInteraction() { Name = atm.Name, Hash = atm.Hash, Position = atm.Position, Rotation = atm.Rotation };
                ATMS.atmInteractions.Add(newAtm);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} ATMs geladen!");
            count = 0;
            foreach (var garage in garageList)
            {
                count++;
                var newGarage = new GarageInteractions.GarageInteraction() { Name = garage.Name, garageId = garage.garageId, Pos = garage.Pos };
                GarageInteractions.garageList.Add(newGarage);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Garagen geladen!");
            count = 0;
            foreach (var shop in itemshopList)
            {
                count++;
                var newShop = new itemShopInteraction.Shop() { name = shop.shopName, Pos = shop.Pos, shopId = shop.shopId, shopItems = shop.shopItems };
                itemShopInteraction.shopList.Add(newShop);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Shops geladen!");
            count = 0;
            foreach (var arena in paintballArenaList)
            {
                count++;
                var newArena = new paintballInteraction.paintBall() { name = arena.arenaName, playerCount = arena.playerCount, playerMax = arena.playerMax, price = arena.price, arenaId = arena.arenaId, spawnPoints = arena.spawnPoints };
                paintballInteraction.paintballArenas.Add(newArena);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Paintball Arenen geladen!");
            count = 0;
            foreach (var fuelstation in fuelstationList)
            {
                count++;
                var newFuelStation = new fuelStationInteract.fuelStation() { name = fuelstation.name, Pos = fuelstation.pos };
                fuelStationInteract.fuelStationList.Add(newFuelStation);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Tankstellen geladen!");
            count = 0;
            foreach (var clothingStore in clothingstoreList)
            {
                count++;
                var newClothingStore = new ClothingStoreInteraction.ClothingStore() { name = clothingStore.name, Pos = clothingStore.Pos, storeType = clothingStore.storeType };
                ClothingStoreInteraction.clothingStores.Add(newClothingStore);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Kleidungsshops geladen!");
            count = 0;
            foreach (var feld in fieldList)
            {
                var dbItem = Main.database.ItemCollection.AsQueryable().FirstOrDefault(i => i.itemId == feld.itemId);
                count++;
                var newField = new feldInteraction.feld() { name = feld.name, pos = feld.pos, id = feld.id, itemId = feld.itemId, itemMax = feld.itemMax, itemMin = feld.itemMin, radius = feld.radius, itemName = dbItem.name };
                feldInteraction.feldListe.Add(newField);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Felder geladen!");
            count = 0;
            foreach (var fraknpc in frakNPCList)
            {
                count++;
                var newFrakNPC = new FrakNPCInteraction.FrakNPC() { frakId = fraknpc.frakId, frakShopItems = fraknpc.frakShopItems, Pos = fraknpc.pos };
                FrakNPCInteraction.frakNPCList.Add(newFrakNPC);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Fraktions NPCs geladen!");
            count = 0;
            foreach (var vehshop in carshops)
            {
                count++;
                var newVehShop = new VehShopInteraction.VehShop() {Cars = vehshop.Cars, Name = vehshop.Name, Visible = vehshop.Visible, Pos = vehshop.Pos, Id = vehshop.Id};
                VehShopInteraction.vehShops.Add(newVehShop);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Vehicle Shops geladen!");
            count = 0;
            foreach (var frakGarage in frakgarages)
            {
                count++;
                var newFrakGarage = new FrakGarageInteraction.FrakGarage() {FrakId = frakGarage.FrakId, Pos = frakGarage.Pos, Id = frakGarage.Id};
                FrakGarageInteraction.frakGarages.Add(newFrakGarage);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} FrakGaragen geladen!");
            count = 0;
            foreach (var store in ammunations)
            {
                count++;
                var newStore = new AmmunationInteraction.Ammunation() {Inventory = store.Inventory, Name = store.Name, Pos = store.Pos, Id = store.Id};
                AmmunationInteraction.AmmunationList.Add(newStore);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Ammunations geladen!");
            count = 0;
            foreach (var kleiderschrank in kleiderschränke)
            {
                count++;
                var newSchrank = new KleiderSchrankInteraction.Kleiderschrank(){Id = kleiderschrank.Id, Range = kleiderschrank.Range, Position = kleiderschrank.Pos };
                KleiderSchrankInteraction.KlederSchränke.Add(newSchrank);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Kleiderschränke geladen!");
            count = 0;
            foreach (var point in jumppoints)
            {
                count++;
                var newPoint = new JumpPointInteraction.JumpPoint()
                    {Position = point.Pos, TargetPosition = point.TargetPosition};
                JumpPointInteraction.JumpPointList.Add(newPoint);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} Jumppoints geladen!");
            count = 0;
            foreach (var kleiderschrank in frakkleiderschränke)
            {
                count++;
                var frakkleiderschrank = new FrakKleiderSchrankInteraction.FrakKleiderschrank()
                {
                    Position = kleiderschrank.Pos,
                    Clothing = kleiderschrank.Clothing,
                    FrakId = kleiderschrank.FrakId
                };
                FrakKleiderSchrankInteraction.KlederSchränke.Add(frakkleiderschrank);
            }
            Console.WriteLine($"[SimpleRoleplay] {count} FraktionsKleiderschränke geladen!");
            count = 0;
            foreach (var itemproducer in itemproducers)
            {
                count++;
                var newProducer = new ItemProductionInteraction.ItemProduction()
                {
                    Id = itemproducer.Id,
                    Position = itemproducer.Pos,
                    IsVisible = itemproducer.IsVisible,
                    Name = itemproducer.Name,
                    NeededItemAmount = itemproducer.NeededItemAmount,
                    NeededItemId = itemproducer.NeededItemId,
                    OutComeAmount = itemproducer.OutComeAmount,
                    OutComeItemId = itemproducer.OutComeItemId
                };
                ItemProductionInteraction.ItemProductionList.Add(newProducer);
            }
            BuergerBueros.buergerBueroList.Add(buergerBuero);
            paintballInteraction.paintballNPCs.Add(paintballNPC);
            return Task.CompletedTask;
        }

        [ClientEvent("client:garage:ausparken")]
        public async Task ausparken(User player, string model, string license, string objId)
        {

            var dbCar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.numPlate == license);

            var ausparkPunkte = Main.database.GarageCollection.AsQueryable().FirstOrDefault(g => g.garageId == dbCar.garageId).Ausparkpunkte;


            for (int i = 0; i < ausparkPunkte.Count; i++)
            {
                var ausparkPunkt = ATMS.atmInteraction.toAltPos(ausparkPunkte.ElementAt(i).position);
                var ausparkRotation = ausparkPunkte.ElementAt(i).rotation;
                var ausparkBlocked = Alt.GetAllVehicles().ToList().FirstOrDefault(v => v.Position.IsInRange(ausparkPunkt, 1.5f));
                var dbModifiers = Main.database.VehicleModiferCollection.AsQueryable().FirstOrDefault(c => c.ModelName.ToLower() == dbCar.carmodel.ToLower());
                if(ausparkBlocked != null)
                {
                    
                }
                else
                {
                    Car spawnCar = (Car)Alt.CreateVehicle(model, ausparkPunkt, new Rotation(0, 0, (float)ausparkRotation));
                    spawnCar.NumberplateText = license;


                    spawnCar.ownerId = dbCar.ownerId;
                    spawnCar.allowedIds = dbCar.allowedIds;
                    spawnCar.carmodel = model;
                    spawnCar.numPlate = license;
                    spawnCar.fuel = dbCar.fuel;
                    spawnCar.kilometer = dbCar.kilometer;
                    spawnCar.carId = dbCar.carId;
                    spawnCar.ManualEngineControl = true;
                    spawnCar.LockState = VehicleLockState.Locked;


                    dbCar.parkedIn = false;
                    await Main.database.CarCollection.ReplaceOneAsync(c => c.numPlate == license, dbCar);
                    if(dbModifiers != null && dbModifiers.VehSpeedModifier != 0)
                    {
                        Alt.EmitAllClients("vehicle:setSpeed", spawnCar, dbModifiers.VehSpeedModifier);
                    }
                    

                    break;
                }

                if (i != ausparkPunkte.Count || !ausparkBlocked.Exists) continue;
                await NotifyHandler.SendNotification(player, "Es sind keine Ausparkpunkte frei!");
                break;
            }
        }

        [ClientEvent("client:garage:einparken")]
        public Task einparken(User player, string model, string license, string objId, int garageId)
        {   
            var carpool = Alt.GetAllVehicles();
            // ReSharper disable once PossibleInvalidCastExceptionInForeachLoop
            foreach (Car car in carpool)
            {
                
                if(car.numPlate == license)
                {
                    car.Remove();
                    var dbCar = Main.database.CarCollection.AsQueryable().FirstOrDefault(c => c.numPlate == car.numPlate);
                    dbCar.parkedIn = true;
                    dbCar.garageId = garageId;
                    dbCar.fuel = car.fuel;
                    dbCar.kilometer = car.kilometer;
                    Main.database.CarCollection.ReplaceOne(c => c.numPlate == car.numPlate, dbCar);
                }
            }
            return Task.CompletedTask;
        }

        public static async Task LoadVehicleShops()
        {
            foreach (var vehShop in Main.database.VehShopCollection.AsQueryable().ToList())
            {
                foreach (var car in vehShop.Cars)
                {
                    var vehSpawn = Alt.CreateVehicle(car.Model, ATMS.atmInteraction.toAltPos(car.Pos), car.Rotation.ToAltPos());
                    vehSpawn.LockState = VehicleLockState.Locked;
                    vehSpawn.NumberplateText = car.Model;
                }
            }
            return;
        }
    }
}