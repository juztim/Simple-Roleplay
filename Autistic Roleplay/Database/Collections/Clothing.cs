

using System.Collections;

namespace Simple_Roleplay.Database.Collections
{
    public class Clothing
    {
        public Component Mask { get; set; } 
        public Component Torso { get; set; } 
        public Component Legs { get; set; } 
        public Component Bag {get; set;} 
        public Component Shoes { get; set; } 
        public Component Accessoires { get; set; }
        public Component Undershirt { get; set; }
        public Component Armor { get; set; }
        public Component Decals { get; set; }
        public Component Tops { get; set; }
        public Component Hats { get; set; }
        public Component Glasses { get; set; }
        public Component Ears { get; set; }
        public Component Watches { get; set; }
        public Component Bracelets { get; set; }


        public IEnumerator GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }
}
