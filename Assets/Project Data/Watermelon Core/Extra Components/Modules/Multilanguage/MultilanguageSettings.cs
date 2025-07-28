using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon
{
    [SetupTab("Multilanguage", texture = "icon_language")]
    [CreateAssetMenu(fileName = "Multilanguage Settings", menuName = "Settings/Multilanguage Settings")]
    public class MultilanguageSettings : ScriptableObject
    {
        [SerializeField] Language defaultLanguage = Language.English;
        public Language DefaultLanguage => defaultLanguage;

        [SerializeField] LanguagePack[] languagePacks;
        public LanguagePack[] LanguagePacks => languagePacks;

        [SerializeField] string[] wordKeys;
        public string[] WordKeys => wordKeys;

        [SerializeField] string[] fontKeys;
        public string[] FontKeys => fontKeys;

        public bool IsLanguageActive(Language language)
        {
            return System.Array.FindIndex(languagePacks, x => x.Language == language) != -1;
        }

        public int GetWordIndex(string key)
        {
            return System.Array.FindIndex(wordKeys, x => x == key);
        }

        public int GetFontIndex(string key)
        {
            return System.Array.FindIndex(fontKeys, x => x == key);
        }

        public LanguagePack GetLanguagePack(Language language)
        {
            LanguagePack languagePack = System.Array.Find(languagePacks, x => x.Language == language);
            if(languagePack != null)
            {
                return languagePack;
            }

            Debug.LogWarning(string.Format("[Multilanguage]: Language {0} is missng! Loading default language.", language));

            return System.Array.Find(languagePacks, x => x.Language == defaultLanguage);
        }
        
        public IEnumerable<Language> GetActiveLanguages()
        {
            if(!languagePacks.IsNullOrEmpty())
            {
                for (int i = 0; i < languagePacks.Length; i++)
                {
                    yield return languagePacks[i].Language;
                }
            }
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------