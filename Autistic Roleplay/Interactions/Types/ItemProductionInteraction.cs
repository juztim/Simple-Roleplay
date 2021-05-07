using System;
using System.Collections.Generic;
using System.Text;
using AltV.Net;

namespace Simple_Roleplay.Interactions.Types
{
    public class ItemProductionInteraction : IScript
    {
        public class ItemProduction
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public Pos Position { get; set; }
            public bool IsVisible { get; set; }
            public int NeededItemId { get; set; }
            public int NeededItemAmount { get; set; }
            public int OutComeItemId { get; set; }
            public int OutComeAmount { get; set; }
        }
        public static IList<ItemProduction> ItemProductionList { get; set; } = new List<ItemProduction>();
        
    }
}
