using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class ClothingStoreInteraction
    {
        public class ClothingStore
        {
            public int storeType { get; set; }
            public Pos Pos { get; set; }
            public string name { get; set; }
        }
        public static IList<ClothingStore> clothingStores { get; set; } = new List<ClothingStore>();
    }
}
