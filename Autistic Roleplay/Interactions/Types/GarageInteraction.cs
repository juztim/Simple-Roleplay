using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class GarageInteractions
    {
        public class GarageInteraction
        {
            public int garageId { get; set; }
            public string Name { get; set; }
            public Pos Pos { get; set; }

        }

        public static List<GarageInteraction> garageList = new List<GarageInteraction>();
    }
}
