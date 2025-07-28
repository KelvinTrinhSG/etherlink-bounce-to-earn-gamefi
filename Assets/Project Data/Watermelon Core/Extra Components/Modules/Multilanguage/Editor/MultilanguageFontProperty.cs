using UnityEditor;
using UnityEngine;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(MultilanguageFontAttribute))]
    public class MultilanguageFontProperty : UnityEditor.PropertyDrawer
    {
        private bool isInited = false;
        private bool hasError = false;
        private string[] fontKeys;

        private void Init(SerializedProperty property)
        {
            MultilanguageInitModule multilanguageInitModule = EditorUtils.GetAsset<MultilanguageInitModule>();

            if (multilanguageInitModule != null)
            {
                MultilanguageSettings multilanguageSettings = multilanguageInitModule.MultilanguageSettings;
                if (multilanguageSettings != null)
                {
                    fontKeys = multilanguageSettings.FontKeys;

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
                int selectedFontId = 0;

                if (string.IsNullOrEmpty(propertyValue))
                {
                    property.stringValue = "";
                    selectedFontId = 0;
                }
                else
                {
                    int foundedKey = System.Array.FindIndex(fontKeys, x => x == property.stringValue);

                    if (foundedKey != -1)
                    {
                        selectedFontId = foundedKey;
                    }
                    else
                    {
                        property.stringValue = "";
                        selectedFontId = 0;
                    }
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginProperty(position, label, property);

                position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                var amountRect = new Rect(position.x, position.y, position.width, position.height);

                selectedFontId = EditorGUI.Popup(amountRect, selectedFontId, fontKeys);

                EditorGUI.indentLevel = indent;

                EditorGUI.EndProperty();

                if (GUI.changed)
                {
                    if (fontKeys[selectedFontId] == "None")
                    {
                        property.stringValue = "";
                    }
                    else
                    {
                        property.stringValue = fontKeys[selectedFontId];
                    }
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