using AltV.Net.Data;
using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class ServerNPCS
    {
        public class ServerNPC
        {
            public int id { get; set; }
            public Position pos { get; set; }
            public string clientEvent { get; set; }

        }

        public static List<ServerNPC> ServerNPCList = new List<ServerNPC>();
    }

}
