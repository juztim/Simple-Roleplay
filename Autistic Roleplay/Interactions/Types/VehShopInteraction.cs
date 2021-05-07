using System;
using System.Collections.Generic;
using System.Text;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Interactions.Types
{
    class VehShopInteraction
    {
        public class VehShop
        {
            public Pos Pos { get; set; }
            public IList<VehShopCar> Cars { get; set; } = new List<VehShopCar>();
            public string Name { get; set; }
            public bool Visible { get; set; }
            public int Id { get; set; }
            
        }
        public static List<VehShop> vehShops { get; set; } = new List<VehShop>();
    }
}
