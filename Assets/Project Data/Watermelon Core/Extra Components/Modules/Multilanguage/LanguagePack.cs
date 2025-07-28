using UnityEngine;

namespace Watermelon
{
    public class LanguagePack : ScriptableObject
    {
        [SerializeField] Language language;
        public Language Language => language;

        [SerializeField] string[] words;
        public string[] Words => words;

        [SerializeField] Font[] fonts;
        public Font[] Fonts => fonts;

        public string GetWord(int index)
        {
            if(words.IsInRange(index))
            {
                return words[index];
            } 

            return string.Empty;
        }
        
        public Font GetFont(int index)
        {
            if (fonts.IsInRange(index))
            {
                return fonts[index];
            }

            return null;
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------
