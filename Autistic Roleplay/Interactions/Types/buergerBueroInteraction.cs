using AltV.Net.Data;
using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class BuergerBueros
    {
        public class BuergerBuero
        {
            public int id { get; set; }
            public Position pos { get; set; }
            public string clientEvent { get; set; }

        }

        public static List<BuergerBuero> buergerBueroList = new List<BuergerBuero>();
    }
}
