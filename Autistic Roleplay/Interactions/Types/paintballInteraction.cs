using AltV.Net.Data;
using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class paintballInteraction
    {
        public class paintBall
        {
            public string name { get; set; }
            public int price { get; set; }
            public int playerCount { get; set; }
            public int playerMax { get; set; }
            public int arenaId { get; set; }
            public IList<Pos> spawnPoints { get; set; } = new List<Pos>();
        }
        public class paintballNPC
        {
            public Position pos { get; set; }
        }
        public static IList<paintballNPC> paintballNPCs { get; set; } = new List<paintballNPC>();
        public static IList<paintBall> paintballArenas { get; set; } = new List<paintBall>();
    }
}
