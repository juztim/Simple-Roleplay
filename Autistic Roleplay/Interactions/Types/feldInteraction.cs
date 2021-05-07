using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class feldInteraction
    {
        public class feld
        {
            public int id { get; set; }
            public string name { get; set; }
            public float radius { get; set; }
            public Pos pos { get; set; }
            public int itemId { get; set; }
            public int itemMin { get; set; }
            public int itemMax { get; set; }
            public string itemName { get; set; }
        }
        public static IList<feld> feldListe { get; set; } = new List<feld>();
    }
}
