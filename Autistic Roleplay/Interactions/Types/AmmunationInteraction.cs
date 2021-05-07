using System;
using System.Collections.Generic;
using System.Text;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Interactions.Types
{
    public class AmmunationInteraction
    {
        public class Ammunation
        {
            public Pos Pos { get; set; }
            public IList<Weapon> Inventory { get; set; } = new List<Weapon>();
            public string Name { get; set; }
            public int Id { get; set; }
        }
        public static IList<Ammunation> AmmunationList { get; set; } = new List<Ammunation>();
    }
}
