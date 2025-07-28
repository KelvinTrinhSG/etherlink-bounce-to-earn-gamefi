using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(MultilanguageWordAttribute))]
    public class MultilanguageWordProperty : UnityEditor.PropertyDrawer
    {
        private bool isInited = false;
        private bool hasError = false;

        private string[] enumWords;

        private List<string> wordsKeys;

        private const int MAX_WORD_LENGTH = 30;

        private MultilanguageSettings multilanguageSettings;

        private void Init(SerializedProperty property)
        {
            MultilanguageInitModule multilanguageInitModule = EditorUtils.GetAsset<MultilanguageInitModule>();

            if (multilanguageInitModule != null)
            {
                multilanguageSettings = multilanguageInitModule.MultilanguageSettings;
                if (multilanguageSettings != null)
                {
                    wordsKeys = new List<string>();

                    Dictionary<string, string> words = Multilanguage.GetWords(multilanguageSettings, multilanguageSettings.DefaultLanguage);

                    MultilanguageWordAttribute wordAttribute = attribute as MultilanguageWordAttribute;

                    wordsKeys = words.Keys.ToList();

                    if (!string.IsNullOrEmpty(wordAttribute.Filter))
                    {
                        wordsKeys = wordsKeys.FindAll(x => x.Contains(wordAttribute.Filter));
                    }

                    int wordsCount = wordsKeys.Count;
                    enumWords = new string[wordsCount];
                    for (int i = 0; i < wordsCount; i++)
                    {
                        string word = words[wordsKeys[i]].Replace("/", "\\");
                        if (word.Length > MAX_WORD_LENGTH)
                            word = word.Substring(0, MAX_WORD_LENGTH);

                        enumWords[i] = word + " - (" + wordsKeys[i] + ")";
                    }

                    hasError = false;
                }
                else
                {
                    hasError = true;

                    Debug.LogWarning("[Multilanguage]: Settings is missing!");
                }
            }
            else
            {
                hasError = true;

                Debug.LogWarning("[Multilanguage]: Module isn't initialised!");
            }

            isInited = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!isInited)
                Init(property);

            if (!hasError)
            {
                string propertyValue = property.stringValue;
                int selectedWordId = 0;

                if (string.IsNullOrEmpty(propertyValue))
                {
                    property.stringValue = null;
                    selectedWordId = -1;
                }
                else
                {
                    int foundedKey = wordsKeys.FindIndex(x => x == property.stringValue);

                    if (foundedKey != -1)
                    {
                        selectedWordId = foundedKey;
                    }
                    else
                    {
                        property.stringValue = "Null";
                        selectedWordId = -1;
                    }
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                var amountRect = new Rect(position.x, position.y, position.width, position.height);

                selectedWordId = EditorGUI.Popup(amountRect, selectedWordId, enumWords);

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();

                if (EditorGUI.EndChangeCheck())
                {
                    property.stringValue = wordsKeys[selectedWordId];
                }
            }
            else
            {
                EditorGUI.LabelField(position, "ERROR");
            }
        }
    }
}

// -----------------
// Multilanguage v 1.0
// -----------------