﻿using AltV.Net;
using Simple_Roleplay.Database.Collections;
using MongoDB.Driver;
using System;
using ItemProductionCollection = Simple_Roleplay.Database.Collections.ItemProductionCollection;

namespace Simple_Roleplay.Database
{
    public class Database
    {
        public IMongoDatabase dataBase;

        //Datenbank Tables
        public IMongoCollection<Users> UserCollection;
        public IMongoCollection<Characters> CharacterCollection;
        public IMongoCollection<ATMCollection> AtmCollection;
        public IMongoCollection<Carcollection> CarCollection;
        public IMongoCollection<GarageCollection> GarageCollection;
        public IMongoCollection<KontoCollection> BankCollection;
        public IMongoCollection<ClothesCollection> ClothesCollection;
        public IMongoCollection<ItemCollection> ItemCollection;
        public IMongoCollection<FractionCollection> FractionCollection;
        public IMongoCollection<Shopcollection> ShopCollection;
        public IMongoCollection<PaintballCollection> PaintballCollection;
        public IMongoCollection<FuelStationCollection> FuelstationCollection;
        public IMongoCollection<ClothingStoreCollection> ClothingStoreCollection;
        public IMongoCollection<feldCollection> FeldCollection;
        public IMongoCollection<FrakNPCCollection> FrakNpcCollection;
        public IMongoCollection<FrakKleiderSchrankCollection> FrakClosetCollection;
        public IMongoCollection<VehShopCollection> VehShopCollection;
        public IMongoCollection<FrakGarageCollection> FrakGarageCollection;
        public IMongoCollection<frakCarCollection> FrakcarCollection;
        public IMongoCollection<AmmunitionCollection> AmmunationCollection;
        public IMongoCollection<KleiderSchrankCollection> ClosetCollection;
        public IMongoCollection<TeleporterCollection> JumpPointCollection;
        public IMongoCollection<VehicleModifierCollection> VehicleModiferCollection;
        public IMongoCollection<AnimationCollection> AnimationCollection;
        public IMongoCollection<ItemProductionCollection> ItemProductionCollection;
        public IMongoCollection<AktenCollection> FileCollection;
        public IMongoCollection<ChatCollection> ChatCollection;


        public void InitDB()
        {
            try
            {
                MongoClient mongoClient;
                if (true)
                {
                    //mongoClient = new MongoClient($"mongodb://localhost:27017/?readPreference=primary&appname=MongoDB%20Compass%20Community&ssl=false");
                    mongoClient = new MongoClient("mongodb://<Username>:<Password>@<Host>/?authSource=<authSource>&readPreference=primary&appname=MongoDB%20Compass%20Community&ssl=false");
                }
                //Initialize Collections
                try
                {
                    dataBase = mongoClient.GetDatabase("SimpleRoleplay");
                    UserCollection = dataBase.GetCollection<Users>("users");
                    CharacterCollection = dataBase.GetCollection<Characters>("characters");
                    AtmCollection = dataBase.GetCollection<ATMCollection>("atmcollection");
                    CarCollection = dataBase.GetCollection<Carcollection>("carcollection");
                    GarageCollection = dataBase.GetCollection<GarageCollection>("garagecollection");
                    BankCollection = dataBase.GetCollection<KontoCollection>("kontocollection");
                    ClothesCollection = dataBase.GetCollection<ClothesCollection>("clothesCollection");
                    ItemCollection = dataBase.GetCollection<ItemCollection>("itemCollection");
                    FractionCollection = dataBase.GetCollection<FractionCollection>("FractionCollection");
                    ShopCollection = dataBase.GetCollection<Shopcollection>("shopCollection");
                    PaintballCollection = dataBase.GetCollection<PaintballCollection>("paintballCollection");
                    FuelstationCollection = dataBase.GetCollection<FuelStationCollection>("fuelstationCollection");
                    ClothingStoreCollection = dataBase.GetCollection<ClothingStoreCollection>("clothingstoreCollection");
                    FeldCollection = dataBase.GetCollection<feldCollection>("feldCollection");
                    FrakNpcCollection = dataBase.GetCollection<FrakNPCCollection>("frakNPCCollection");
                    FrakClosetCollection = dataBase.GetCollection<FrakKleiderSchrankCollection>("frakKleiderschrankCollection");
                    VehShopCollection = dataBase.GetCollection<VehShopCollection>("vehShopCollection");
                    FrakGarageCollection = dataBase.GetCollection<FrakGarageCollection>("frakGarageCollection");
                    FrakcarCollection = dataBase.GetCollection<frakCarCollection>("frakCarCollection");
                    AmmunationCollection = dataBase.GetCollection<AmmunitionCollection>("ammunationCollection");
                    ClosetCollection = dataBase.GetCollection<KleiderSchrankCollection>("ClosetCollection");
                    JumpPointCollection = dataBase.GetCollection<TeleporterCollection>("jumpPointCollection");
                    VehicleModiferCollection = dataBase.GetCollection<VehicleModifierCollection>("vehicleModifierCollection");
                    AnimationCollection = dataBase.GetCollection<AnimationCollection>("animationCollection");
                    ItemProductionCollection = dataBase.GetCollection<ItemProductionCollection>("itemProductionCollection");
                    FileCollection = dataBase.GetCollection<AktenCollection>("aktenCollection");
                    ChatCollection = dataBase.GetCollection<ChatCollection>("chatCollection");


                    Console.WriteLine("[SimpleRoleplay] Datenbank verbunden!");
                }
                catch (Exception e)
                {
                    Alt.Log("ERROR IN DATABASE INIT: " + e);
                }
            }
            catch (TimeoutException)
            {
                throw new MongoConnectionException(null, "Datenbank nicht erreichbar!");
            }
        }



        
    }
}
