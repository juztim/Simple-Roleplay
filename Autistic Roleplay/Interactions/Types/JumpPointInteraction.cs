using System;
using System.Collections.Generic;
using System.Text;

namespace Simple_Roleplay.Interactions.Types
{
    public class JumpPointInteraction
    {
        public class JumpPoint
        {
            public Pos Position { get; set; }
            public Pos TargetPosition { get; set; }
        }
        public static IList<JumpPoint> JumpPointList { get; set; } = new List<JumpPoint>();
    }
}
