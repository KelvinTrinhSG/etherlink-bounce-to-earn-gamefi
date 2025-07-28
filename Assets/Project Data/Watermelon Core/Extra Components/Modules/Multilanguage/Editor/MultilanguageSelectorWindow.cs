using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Watermelon
{
    sealed internal class MultilanguageSelectorWindow : EditorWindow
    {
        private Language[] languages;

        private MultilanguageSettings multilanguageSettings;

        private int selectedLanguage;

        [MenuItem("Tools/Editor/Multilanguage")]
        private static void OpenWindow()
        {
            MultilanguageSelectorWindow window = (MultilanguageSelectorWindow)GetWindow(typeof(MultilanguageSelectorWindow), true, "Multilanguage Debugger");
            window.Show();
        }

        private void OnEnable()
        {
            MultilanguageInitModule multilanguageInitModule = EditorUtils.GetAsset<MultilanguageInitModule>();

            if(multilanguageInitModule != null)
            {
                multilanguageSettings = multilanguageInitModule.MultilanguageSettings;
                if(multilanguageSettings != null)
                {
                    languages = multilanguageSettings.GetActiveLanguages().ToArray();

                    selectedLanguage = PlayerPrefs.GetInt(Multilanguage.PREFS_KEY_NAME, (int)multilanguageSettings.DefaultLanguage);
                }
                else
                {
                    Debug.LogWarning("[Multilanguage]: Settings is missing!");
                }
            }
            else
            {
                Debug.LogWarning("[Multilanguage]: Module isn't initialised!");
            }
        }

        public void OnGUI()
        {
            GUILayout.Space(12);

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);
            for (int i = 0; i < languages.Length; i++)
            {
                int languageValue = (int)languages[i];

                GUIStyle guiStyle = EditorStylesExtended.button_01;
                if (languageValue == selectedLanguage)
                    guiStyle = EditorStylesExtended.button_03;

                if (GUILayout.Button(languages[i].ToString() + (multilanguageSettings.DefaultLanguage == languages[i] ? " (default)" : ""), guiStyle))
                {
                    selectedLanguage = languageValue;

                    if (Application.isPlaying)
                    {
                        Multilanguage.SetLanguage(languages[i]);
                    }
                    else
                    {
                        PlayerPrefs.SetInt(Multilanguage.PREFS_KEY_NAME, languageValue);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------