using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple_Roleplay.Interactions.Types
{
    public class KleiderSchrankInteraction 
    {
        public class Kleiderschrank
        {
            public int Id { get; set; }
            public Pos Position { get; set; }
            public float Range { get; set; }
        }
        public static IList<Kleiderschrank> KlederSchränke { get; set; } = new List<Kleiderschrank>();
    }

    public class FrakKleiderSchrankInteraction
    {
        public class FrakKleiderschrank
        {
            public int FrakId { get; set; }
            public Pos Position { get; set; }
            public IList<int> Clothing { get; set; }
        }
        public static IList<FrakKleiderschrank> KlederSchränke { get; set; } = new List<FrakKleiderschrank>();
    }
}
