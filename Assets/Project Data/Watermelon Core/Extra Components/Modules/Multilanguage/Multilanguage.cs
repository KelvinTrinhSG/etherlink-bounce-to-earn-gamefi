#pragma warning disable 0649

using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public sealed class Multilanguage
    {
        /// <summary>
        /// Player prefs language key
        /// </summary>
        public const string PREFS_KEY_NAME = "settings.Language";
                
        public static OnLanguageChangedCallback OnLanguageChanged;

        private static readonly Dictionary<Language, Language> linkLanguages = new Dictionary<Language, Language>()
        {
            //CIS Countries
            { Language.Belarusian, Language.Russian  },

            { Language.Catalan, Language.Spanish  },
            { Language.Chinese, Language.ChineseSimplified }
        };

        /// <summary>
        /// Current language
        /// </summary>
        private static Language currentLanguage;
        public static Language CurrentLanguage
        {
            get { return currentLanguage; }
        }

        private static Dictionary<string, string> loadedWords = new Dictionary<string, string>();
        private static Dictionary<string, Font> loadedFonts = new Dictionary<string, Font>();

        private static MultilanguageSettings multilanguageSettings;

        public void Init(MultilanguageSettings multilanguageSettings)
        {
#if UNITY_EDITOR
            if(multilanguageSettings.LanguagePacks.IsNullOrEmpty())
            {
                Debug.LogError("[Multilanguage]: There are no active languages!");

                return;
            }
#endif

            // Setup settings
            Multilanguage.multilanguageSettings = multilanguageSettings;

            //On start check if any language saved in player prefs
            if (!PlayerPrefs.HasKey(PREFS_KEY_NAME))
            {
                Language systemSelectedLanguage = LangUtils.GetSystemLanguage();

                if (linkLanguages.ContainsKey(systemSelectedLanguage))
                    systemSelectedLanguage = linkLanguages[systemSelectedLanguage];

                if (!IsLanguageActive(systemSelectedLanguage))
                    systemSelectedLanguage = multilanguageSettings.DefaultLanguage;

                SetLanguage(systemSelectedLanguage);
            }
            else
            {
                Language projectLanguages = (Language)PlayerPrefs.GetInt(PREFS_KEY_NAME);

                if (!IsLanguageActive(projectLanguages))
                    projectLanguages = multilanguageSettings.DefaultLanguage;

                SetLanguage(projectLanguages);
            }
        }

        /// <summary>
        /// Get word by the key
        /// </summary>
        public static string GetWord(string key)
        {
            if (key == string.Empty)
            {
                return string.Empty;
            }
            else
            {
                return loadedWords[key];
            }
        }

        /// <summary>
        /// Get font by the name
        /// </summary>
        public static Font GetFont(string name)
        {
            if (loadedFonts.ContainsKey(name))
            {
                return loadedFonts[name];
            }

            return null;
        }

        /// <summary>
        /// Change current language
        /// </summary>
        public static void SetLanguage(Language language)
        {
            if (IsLanguageActive(language))
            {
                Debug.Log("[Multilanguage]: Selected language - " + language.ToString());

                PlayerPrefs.SetInt(PREFS_KEY_NAME, (int)language);

                currentLanguage = language;

                LoadResources(language);

                if (OnLanguageChanged != null)
                    OnLanguageChanged.Invoke();
            }
        }

        public static void ChangeLanguage(Language language)
        {
            SetLanguage(language);
        }

        /// <summary>
        /// Load words and audio from resources 
        /// </summary>
        private static void LoadResources(Language language)
        {
            //Load words
            loadedWords = GetWords(multilanguageSettings, language);
            
            Debug.Log("[Multilanguage]: Loaded " + loadedWords.Count + " words!");

            //Load fonts
            loadedFonts = GetFonts(multilanguageSettings, language);

            Debug.Log("[Multilanguage]: Loaded " + loadedFonts.Count + " fonts!");
        }

        public static Dictionary<string, Font> GetFonts(MultilanguageSettings multilanguageSettings, Language language)
        {
            Dictionary<string, Font> fontsDictionary = new Dictionary<string, Font>();

            LanguagePack languagePack = multilanguageSettings.GetLanguagePack(language);

            string[] wordKeys = multilanguageSettings.FontKeys;
            Font[] fonts = languagePack.Fonts;

            int wordsCount = wordKeys.Length;
            for (int i = 0; i < wordsCount; i++)
            {
                fontsDictionary.Add(wordKeys[i], fonts[i]);
            }

            return fontsDictionary;
        }

        public static Dictionary<string, string> GetWords(MultilanguageSettings multilanguageSettings, Language language)
        {
            Dictionary<string, string> wordsDictionary = new Dictionary<string, string>();

            LanguagePack languagePack = multilanguageSettings.GetLanguagePack(language);

            string[] wordKeys = multilanguageSettings.WordKeys;
            string[] words = languagePack.Words;

            int wordsCount = wordKeys.Length;
            for(int i = 0; i < wordsCount; i++)
            {
                wordsDictionary.Add(wordKeys[i], words[i]);
            }

            return wordsDictionary;
        }

        /// <summary>
        /// Get language and check if it exist
        /// </summary>
        private static bool IsLanguageActive(Language language)
        {
            if (!multilanguageSettings.IsLanguageActive(language))
            {
                //Debug.LogError("Language " + language.ToString() + " doesn't exist!");

                return false;
            }

            return true;
        }

        public delegate void OnLanguageChangedCallback();
    }

    public enum Language
    {
        Afrikaans = 0,
        Arabic = 1,
        Basque = 2,
        Belarusian = 3,
        Bulgarian = 4,
        Catalan = 5,
        Chinese = 6,
        Czech = 7,
        Danish = 8,
        Dutch = 9,
        English = 10,
        Estonian = 11,
        Faroese = 12,
        Finnish = 13,
        French = 14,
        German = 15,
        Greek = 16,
        Hebrew = 17,
        Hugarian = 18,
        Hungarian = 18,
        Icelandic = 19,
        Indonesian = 20,
        Italian = 21,
        Japanese = 22,
        Korean = 23,
        Latvian = 24,
        Lithuanian = 25,
        Norwegian = 26,
        Polish = 27,
        Portuguese = 28,
        Romanian = 29,
        Russian = 30,
        SerboCroatian = 31,
        Slovak = 32,
        Slovenian = 33,
        Spanish = 34,
        Swedish = 35,
        Thai = 36,
        Turkish = 37,
        Ukrainian = 38,
        Vietnamese = 39,
        ChineseSimplified = 40,
        ChineseTraditional = 41,
        Hindi = 42,
        Telugu = 43,
        Bangla = 44,
        Unknown = 45
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------

// Changelog
// v 1.0
// • Reworked words save/load logic
// v 0.4
// • remove MODULE_MULTILANGUAGE define
// v 0.3
// • Added link languages
// v 0.2
// • Load words from ScriptableObject (LanguageText), export and import words data
// v 0.1
// • Base multilanguage system