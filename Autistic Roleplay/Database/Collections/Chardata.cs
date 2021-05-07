
using System.Collections.Generic;

namespace Simple_Roleplay.Database.Collections
{
    public class Chardata
    {
		public long? sex { get; set; }
		public long? faceFather { get; set; }
        public long? faceMother { get; set; }
		public long? skinFather { get; set; }
		public long? skinMother { get; set; }
		public float? faceMix { get; set; }
		public float? skinMix { get; set; }
		public IList<float?> structure { get; set; }
		public long? hair { get; set; }
		public long? hairColor1 { get; set; }
		public long? hairColor2 { get; set; }
		public hairOverlay hairOverlay { get; set; }
		public long? facialHair { get; set; }
		public long? facialHairColor1 { get; set; }
		public float? facialHairOpacity { get; set; }
		public long? eyebrows { get; set; }
		public float? eyebrowsOpacity { get; set; }
		public long? eyeBrowsColor1 { get; set; }
		public long? eyes { get; set; }
		public IList<opacityOverlays> opacityOverlays { get; set; }
		public IList<colorOverlays> colorOverlays { get; set; }

	}
}
