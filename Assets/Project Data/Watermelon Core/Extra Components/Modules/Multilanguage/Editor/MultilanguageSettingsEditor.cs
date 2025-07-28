using UnityEngine;
using UnityEditor;
using System.Linq;

namespace Watermelon
{
    [CustomEditor(typeof(MultilanguageSettings))]
    public class MultilanguageSettingsEditor : WatermelonEditor
    {
        private const string DEFAULT_LANGUAGE_PROPERTY_NAME = "defaultLanguage";
        private const string LANGUAGE_PACKS_PROPERTY_NAME = "languagePacks";
        private const string WORD_KEYS_PROPERTY_NAME = "wordKeys";
        private const string FONT_KEYS_PROPERTY_NAME = "fontKeys";

        private Language[] activeLanguages;
        private string[] activeLanguageNames;
        private int activeSystemLanguage = 0;
        private int defaultLanguage = 0;
        
        private MultilanguageSettings multilanguageSettings;

        private SerializedProperty defaultLanguageProperty;
        private SerializedProperty languagePacksProperty;
        private SerializedProperty wordKeysProperty;
        private SerializedProperty fontKeysProperty;

        private SerializedObject selectedLanguagePackObject;

        private bool tempLanguageEnabled = false;
        private Language tempLanguage = Language.Unknown;

        private bool tempWordEnabled = false;
        private string tempWord;

        private bool tempFontEnabled = false;
        private string tempFont;

        private GUIContent addLanguageButton;
        private GUIContent addWordButton;
        private GUIContent addFontButton;
        private GUIContent applyButton;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Get properties
            defaultLanguageProperty = serializedObject.FindProperty(DEFAULT_LANGUAGE_PROPERTY_NAME);
            languagePacksProperty = serializedObject.FindProperty(LANGUAGE_PACKS_PROPERTY_NAME);
            wordKeysProperty = serializedObject.FindProperty(WORD_KEYS_PROPERTY_NAME);
            fontKeysProperty = serializedObject.FindProperty(FONT_KEYS_PROPERTY_NAME);

            // Get Multilanguage settings from target
            multilanguageSettings = target as MultilanguageSettings;

            // Reset temp language
            tempLanguageEnabled = false;
            tempLanguage = Language.Unknown;

            // Initialize languages enums
            Init();
        }

        private void AddActiveLanguage(Language language)
        {
            serializedObject.Update();

            languagePacksProperty.arraySize++;

            LanguagePack languagePack = CreateInstance<LanguagePack>();
            languagePack.name = language.ToString();
            //testInitModule.hideFlags = HideFlags.HideInHierarchy;

            SerializedObject languageSerializedObject = new SerializedObject(languagePack);

            languageSerializedObject.Update();
            languageSerializedObject.FindProperty("language").intValue = (int)language;
            languageSerializedObject.FindProperty("words").arraySize = wordKeysProperty.arraySize;
            languageSerializedObject.ApplyModifiedProperties();

            AssetDatabase.AddObjectToAsset(languagePack, target);
            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(languagePack));

            languagePacksProperty.GetArrayElementAtIndex(languagePacksProperty.arraySize - 1).objectReferenceValue = languagePack;

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveActiveLanguage(int languageIndex)
        {
            SerializedProperty serializedProperty = languagePacksProperty.GetArrayElementAtIndex(languageIndex);

            serializedProperty.serializedObject.Update();

            Object removedObject = serializedProperty.objectReferenceValue;

            languagePacksProperty.RemoveFromObjectArrayAt(languageIndex);

            AssetDatabase.RemoveObjectFromAsset(removedObject);
            AssetDatabase.SaveAssets();

            DestroyImmediate(removedObject, true);

            EditorUtility.SetDirty(target);

            serializedProperty.serializedObject.ApplyModifiedProperties();

            EditorApplication.delayCall += delegate
            {
                Init();
            };
        }

        protected override void Styles()
        {
            // Get styles
            addLanguageButton = new GUIContent(EditorStylesExtended.ICON_SPACE + "Add Language", EditorStylesExtended.GetTexture("icon_add", EditorStylesExtended.IconColor));
            addWordButton = new GUIContent(EditorStylesExtended.ICON_SPACE + "Add Word", EditorStylesExtended.GetTexture("icon_add", EditorStylesExtended.IconColor));
            addFontButton = new GUIContent(EditorStylesExtended.ICON_SPACE + "Add Font", EditorStylesExtended.GetTexture("icon_add", EditorStylesExtended.IconColor));

            applyButton = new GUIContent("Apply");
        }

        private void Init()
        {
            activeLanguages = multilanguageSettings.GetActiveLanguages().ToArray();
            activeLanguageNames = new string[activeLanguages.Length];
            for (int i = 0; i < activeLanguages.Length; i++)
            {
                activeLanguageNames[i] = activeLanguages[i].ToString();
            }

            defaultLanguage = System.Array.FindIndex(activeLanguages, x => x == multilanguageSettings.DefaultLanguage);

            if(activeLanguages.Length > 0)
            {
                activeSystemLanguage = 0;
                selectedLanguagePackObject = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(activeSystemLanguage).objectReferenceValue);
            }
            else
            {
                activeSystemLanguage = -1;
                selectedLanguagePackObject = null;
            }
        }
        
        public override void OnInspectorGUI()
        {
            InitStyles();

            serializedObject.Update();

            Rect editorRect = EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);
            EditorGUILayoutCustom.Header("MULTILANGUAGE");

            EditorGUI.BeginChangeCheck();
            defaultLanguage = EditorGUILayout.Popup("Default Language", defaultLanguage, activeLanguageNames);
            if(EditorGUI.EndChangeCheck())
            {
                defaultLanguageProperty.intValue = (int)multilanguageSettings.LanguagePacks[defaultLanguage].Language;
            }

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Active Languages", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();

            int languagesArraySize = languagePacksProperty.arraySize;
            if (languagesArraySize > 0)
            {
                for (int i = 0; i < languagesArraySize; i++)
                {
                    LanguagePack languagePack = (LanguagePack)languagePacksProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                    if(languagePack != null)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUI.BeginDisabledGroup(true);
                        EditorGUILayout.EnumPopup(GUIContent.none, languagePack.Language);
                        EditorGUI.EndDisabledGroup();

                        if (GUILayout.Button("X", EditorStylesExtended.button_04_mini, GUILayout.Height(18), GUILayout.Width(18)))
                        {
                            if (EditorUtility.DisplayDialog("Remove language", "Are you sure you want to remove language?", "Remove", "Cancel"))
                            {
                                RemoveActiveLanguage(i);

                                return;
                            }
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }

            if(tempLanguageEnabled)
            {
                EditorGUILayout.BeginHorizontal();
                tempLanguage = (Language)EditorGUILayout.EnumPopup(GUIContent.none, tempLanguage, GUILayout.MinWidth(10));

                if (GUILayout.Button("X", EditorStylesExtended.button_04_mini, GUILayout.Height(18), GUILayout.Width(18)))
                {
                    tempLanguageEnabled = false;
                    tempLanguage = Language.Unknown;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if(tempLanguageEnabled)
            {
                if (GUILayout.Button(applyButton, EditorStylesExtended.button_03, GUILayout.Width(120)))
                {
                    if(tempLanguage == Language.Unknown)
                    {
                        EditorUtility.DisplayDialog("Wrong Language", "Please, select language!", "Close");

                        return;
                    }

                    if (System.Array.FindIndex(activeLanguages, x => x == tempLanguage) != -1)
                    {
                        EditorUtility.DisplayDialog("Wrong Language", tempLanguage + " language already exists!", "Close");

                        return;
                    }

                    AddActiveLanguage(tempLanguage);

                    tempLanguageEnabled = false;
                    tempLanguage = Language.Unknown;

                    EditorApplication.delayCall += delegate
                    {
                        Init();
                    };
                }
            }
            else
            {
                if (GUILayout.Button(addLanguageButton, EditorStylesExtended.button_01, GUILayout.Width(120)))
                {
                    tempLanguageEnabled = true;
                    tempLanguage = Language.Unknown;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndVertical();

            GUILayout.Space(8);

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            EditorGUI.BeginChangeCheck();
            activeSystemLanguage = EditorGUILayout.Popup("Selected Language", activeSystemLanguage, activeLanguageNames);
            if (EditorGUI.EndChangeCheck())
            {
                selectedLanguagePackObject = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(activeSystemLanguage).objectReferenceValue);
            }

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            EditorGUILayoutCustom.Header("WORDS");

            if(activeSystemLanguage != -1)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key", EditorStyles.boldLabel);
                GUILayout.Label("Value", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                int wordsArraySize = wordKeysProperty.arraySize;
                for (int i = 0; i < wordsArraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty keySerializedProperty = wordKeysProperty.GetArrayElementAtIndex(i);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(keySerializedProperty, GUIContent.none);
                    EditorGUI.EndDisabledGroup();

                    selectedLanguagePackObject.Update();
                    SerializedProperty wordSerializedProperty = selectedLanguagePackObject.FindProperty("words").GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(wordSerializedProperty, GUIContent.none);
                    selectedLanguagePackObject.ApplyModifiedProperties();

                    if (GUILayout.Button("X", EditorStylesExtended.button_04_mini, GUILayout.Height(18), GUILayout.Width(18)))
                    {
                        int index = i;

                        if (EditorUtility.DisplayDialog("Remove word", string.Format("Are you sure you want to remove '{0}' key?", keySerializedProperty.stringValue), "Remove", "Cancel"))
                        {
                            // Remove word
                            wordKeysProperty.RemoveFromVariableArrayAt(index);

                            // Add key to all languages
                            int activeLanguagesPacksCount = languagePacksProperty.arraySize;
                            for (int j = 0; j < activeLanguagesPacksCount; j++)
                            {
                                SerializedObject languagePack = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(j).objectReferenceValue);

                                languagePack.Update();

                                SerializedProperty wordsSerializedProperty = languagePack.FindProperty("words");
                                wordsSerializedProperty.RemoveFromVariableArrayAt(index);

                                languagePack.ApplyModifiedProperties();

                                languagePack.Dispose();
                            }

                            return;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (tempWordEnabled)
                {
                    EditorGUILayout.BeginHorizontal();

                    tempWord = EditorGUILayout.TextField(tempWord);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.TextField(string.Empty);
                    EditorGUI.EndDisabledGroup();

                    GUILayout.Space(21);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (tempWordEnabled)
            {
                if(GUILayout.Button("Cancel", EditorStylesExtended.button_04))
                {
                    tempWordEnabled = false;
                }

                if (GUILayout.Button(applyButton, EditorStylesExtended.button_03, GUILayout.Width(120)))
                {
                    // Validate word
                    if(string.IsNullOrEmpty(tempWord))
                    {
                        EditorUtility.DisplayDialog("Wrong Key", "Key can't be empty!", "Close");

                        return;
                    }

                    if (multilanguageSettings.GetWordIndex(tempWord) != -1)
                    {
                        EditorUtility.DisplayDialog("Wrong Key", "Key " + tempWord + " already exists!", "Close");

                        return;
                    }

                    // Add word
                    wordKeysProperty.serializedObject.Update();
                    wordKeysProperty.arraySize++;

                    wordKeysProperty.GetArrayElementAtIndex(wordKeysProperty.arraySize - 1).stringValue = tempWord;
                    wordKeysProperty.serializedObject.ApplyModifiedProperties();

                    // Add key to all languages
                    int activeLanguagesPacksCount = languagePacksProperty.arraySize;
                    for (int i = 0; i < activeLanguagesPacksCount; i++)
                    {
                        SerializedObject languagePack = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(i).objectReferenceValue);

                        languagePack.Update();

                        SerializedProperty wordsSerializedProperty = languagePack.FindProperty("words");
                        wordsSerializedProperty.arraySize++;
                        wordsSerializedProperty.GetArrayElementAtIndex(wordsSerializedProperty.arraySize - 1).stringValue = "";

                        languagePack.ApplyModifiedProperties();

                        languagePack.Dispose();
                    }

                    tempWordEnabled = false;
                    tempWord = string.Empty;
                }
            }
            else
            {
                if (GUILayout.Button(addWordButton, EditorStylesExtended.button_01, GUILayout.Width(120)))
                {
                    tempWordEnabled = true;
                    tempWord = string.Empty;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(8);

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            EditorGUILayoutCustom.Header("FONTS");

            if (activeSystemLanguage != -1)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key", EditorStyles.boldLabel);
                GUILayout.Label("Font", EditorStyles.boldLabel);
                EditorGUILayout.EndHorizontal();

                int keysArraySize = fontKeysProperty.arraySize;
                for (int i = 0; i < keysArraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();

                    SerializedProperty keySerializedProperty = fontKeysProperty.GetArrayElementAtIndex(i);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.PropertyField(keySerializedProperty, GUIContent.none);
                    EditorGUI.EndDisabledGroup();

                    selectedLanguagePackObject.Update();
                    SerializedProperty fontSerializedProperty = selectedLanguagePackObject.FindProperty("fonts").GetArrayElementAtIndex(i);
                    EditorGUILayout.PropertyField(fontSerializedProperty, GUIContent.none);
                    selectedLanguagePackObject.ApplyModifiedProperties();

                    if (GUILayout.Button("X", EditorStylesExtended.button_04_mini, GUILayout.Height(18), GUILayout.Width(18)))
                    {
                        int index = i;

                        if (EditorUtility.DisplayDialog("Remove font", string.Format("Are you sure you want to remove '{0}' key?", keySerializedProperty.stringValue), "Remove", "Cancel"))
                        {
                            // Remove word
                            fontKeysProperty.RemoveFromVariableArrayAt(index);

                            // Add key to all languages
                            int activeLanguagesPacksCount = languagePacksProperty.arraySize;
                            for (int j = 0; j < activeLanguagesPacksCount; j++)
                            {
                                SerializedObject languagePack = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(j).objectReferenceValue);

                                languagePack.Update();

                                SerializedProperty fontsSerializedProperty = languagePack.FindProperty("fonts");
                                fontsSerializedProperty.RemoveFromObjectArrayAt(index);

                                languagePack.ApplyModifiedProperties();

                                languagePack.Dispose();
                            }

                            return;
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (tempFontEnabled)
                {
                    EditorGUILayout.BeginHorizontal();

                    tempFont = EditorGUILayout.TextField(tempFont);

                    EditorGUI.BeginDisabledGroup(true);
                    EditorGUILayout.ObjectField(null, typeof(Font), false);
                    EditorGUI.EndDisabledGroup();

                    GUILayout.Space(21);

                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (tempFontEnabled)
            {
                if (GUILayout.Button("Cancel", EditorStylesExtended.button_04))
                {
                    tempFontEnabled = false;
                }

                if (GUILayout.Button(applyButton, EditorStylesExtended.button_03, GUILayout.Width(120)))
                {
                    // Validate word
                    if (string.IsNullOrEmpty(tempFont))
                    {
                        EditorUtility.DisplayDialog("Wrong Key", "Key can't be empty!", "Close");

                        return;
                    }

                    if (multilanguageSettings.GetFontIndex(tempFont) != -1)
                    {
                        EditorUtility.DisplayDialog("Wrong Key", "Key " + tempWord + " already exists!", "Close");

                        return;
                    }

                    // Add word
                    fontKeysProperty.serializedObject.Update();
                    fontKeysProperty.arraySize++;

                    fontKeysProperty.GetArrayElementAtIndex(fontKeysProperty.arraySize - 1).stringValue = tempFont;
                    fontKeysProperty.serializedObject.ApplyModifiedProperties();

                    // Add key to all languages
                    int activeLanguagesPacksCount = languagePacksProperty.arraySize;
                    for (int i = 0; i < activeLanguagesPacksCount; i++)
                    {
                        SerializedObject languagePack = new SerializedObject(languagePacksProperty.GetArrayElementAtIndex(i).objectReferenceValue);

                        languagePack.Update();

                        SerializedProperty wordsSerializedProperty = languagePack.FindProperty("fonts");
                        wordsSerializedProperty.arraySize++;
                        wordsSerializedProperty.GetArrayElementAtIndex(wordsSerializedProperty.arraySize - 1).objectReferenceValue = null;

                        languagePack.ApplyModifiedProperties();

                        languagePack.Dispose();
                    }

                    tempFontEnabled = false;
                    tempFont = string.Empty;
                }
            }
            else
            {
                if (GUILayout.Button(addFontButton, EditorStylesExtended.button_01, GUILayout.Width(120)))
                {
                    tempFontEnabled = true;
                    tempFont = string.Empty;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();

            EditorGUILayoutCustom.DrawCompileWindow(editorRect);
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------