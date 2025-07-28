#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    [RequireComponent(typeof(Text))]
    public class MultilanguageSmartText : MonoBehaviour
    {
        [SerializeField]
        [MultilanguageWord]
        private string key;

        [SerializeField]
        [MultilanguageFont]
        private string font;

        [SerializeField]
        [HideInInspector]
        private MultilanguageFontSizeOverride[] fontSizeOverrides;

        private Text textComponent;
        public Text TextComponent
        {
            get
            {
                if (textComponent == null)
                    textComponent = GetComponent<Text>();

                return textComponent;
            }
        }

        private Language initedLanguage = Language.Unknown;

        private int defaultFontSize;

        private void Awake()
        {
            textComponent = GetComponent<Text>();

            defaultFontSize = textComponent.fontSize;
        }

        private void OnEnable()
        {
            if (initedLanguage != Multilanguage.CurrentLanguage)
            {
                InitWord();
            }

            Multilanguage.OnLanguageChanged += InitWord;
        }

        private void OnDisable()
        {
            Multilanguage.OnLanguageChanged -= InitWord;
        }

        public TextProcessor textProcessor;

        public void InitWord()
        {
            if (textProcessor != null)
                textComponent.text = textProcessor.Invoke(Multilanguage.GetWord(key));
            else
                textComponent.text = Multilanguage.GetWord(key);

            if (!string.IsNullOrEmpty(font))
            {
                Font tempFont = Multilanguage.GetFont(font);
                if(tempFont != null)
                {
                    textComponent.font = tempFont;
                }
            }

            initedLanguage = Multilanguage.CurrentLanguage;

            int fontOverrideIndex = System.Array.FindIndex(fontSizeOverrides, x => x.language == initedLanguage);
            if (fontOverrideIndex != -1)
            {
                textComponent.fontSize = fontSizeOverrides[fontOverrideIndex].fontSize;
            }
            else
            {
                textComponent.fontSize = defaultFontSize;
            }
        }

        public delegate string TextProcessor(string text);
    }

    [System.Serializable]
    public struct MultilanguageFontSizeOverride
    {
        public Language language;
        public int fontSize;
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------
