using System;
using System.Collections.Generic;
using System.Text;
using Simple_Roleplay.Database.Collections;

namespace Simple_Roleplay.Interactions.Types
{
    public class FrakGarageInteraction
    {
        public class FrakGarage
        {
            public int Id { get; set; }
            public Pos Pos { get; set; }
            public int FrakId { get; set; }
            public IList<Ausparkpunkt> Ausparkpunkte { get; set; }
        }

        public static IList<FrakGarage> frakGarages { get; set; } = new List<FrakGarage>();
    }
}
