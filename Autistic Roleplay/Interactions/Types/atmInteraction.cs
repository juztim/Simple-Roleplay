using AltV.Net.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Simple_Roleplay.Interactions.Types
{
    public class ATMS
    {
        public class atmInteraction
        {
            [BsonElement]
            public string Name { get; set; }
            [BsonElement]
            public long Hash { get; set; }
            [BsonElement]
            public Pos Position { get; set; }
            [BsonElement]
            public Rot Rotation { get; set; }

            public static Position toAltPos(Pos position)
            {
                return new Position((float)position.X, (float)position.Y, (float)position.Z);
            } 
        }
        public static List<atmInteraction> atmInteractions = new List<atmInteraction>();
    }
    
    public class Pos
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public Pos(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Position ToAltPos()
        {
            return new Position((float)this.X, (float)this.Y, (float)this.Z);
        }

        
    }
    public class Rot
    {
        public double X;
        public double Y;
        public double Z;

        public Rot(int v1, int v2, float v3)
        {
            this.X = v1;
            this.Y = v2;
            this.Z = v3;
        }


        public Rotation ToAltPos()
        {
            return new Rotation((float)0, (float)0, (float)this.Z);
        }
    }
}
