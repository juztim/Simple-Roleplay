using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class itemShopInteraction
    {
        public class Shop
        {
            public int shopId { get; set; }
            public Pos Pos { get; set; }
            public string name { get; set; }
            public IList<int> shopItems { get; set; } = new List<int>();
        }
        public class ShopClientItem
        {
            public string name { get; set; }
            public int price { get; set; }
            public int itemId { get; set; }
        }
        
        public static List<Shop> shopList = new List<Shop>();
    }
}
