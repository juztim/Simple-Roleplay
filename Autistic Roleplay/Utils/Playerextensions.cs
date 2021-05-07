using AltV.Net.Data;

namespace Simple_Roleplay.Utils
{
    public static class Playerextensions
    {
        public static bool IsInRange(this Position currentPosition, Position otherPosition, float distance)
            => currentPosition.Distance(otherPosition) <= distance;

    }
}
