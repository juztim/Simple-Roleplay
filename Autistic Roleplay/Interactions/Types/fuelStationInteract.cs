using AltV.Net;
using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class fuelStationInteract : IScript
    {
        public class fuelStation
        {
            public string name { get; set; }
            public int id { get; set; }
            public Pos Pos { get; set; }
        }
        public static IList<fuelStation> fuelStationList = new List<fuelStation>();
    }
}
