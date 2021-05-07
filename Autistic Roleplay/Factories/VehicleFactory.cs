using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace Simple_Roleplay.Factories
{
    public class Car : Vehicle
    {
        public int garageId { get; set; }
        public ObjectId ownerObjId { get; set; }
        public string carmodel { get; set; }
        public string numPlate { get; set; }
        public double fuel { get; set; }
        public int fueltype { get; set; }
        public ObjectId objectId { get; set; }
        public double kilometer { get; set; }
        public bool isAdminCar { get; set; }
        public List<int> allowedIds { get; set; }
        public int ownerId { get; set; }
        public int carId { get; set; }
        public int frakId { get; set; }


        public Car(uint model, Position position, Rotation rotation) : base(model, position, rotation)
        {
            
        }

        public Car(IntPtr nativePointer, ushort id) : base(nativePointer, id)
        {

        }
        
    }

    public class VehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(IntPtr nativePointer, ushort id)
        {
            return new Car(nativePointer, id);
        }
    }
}
