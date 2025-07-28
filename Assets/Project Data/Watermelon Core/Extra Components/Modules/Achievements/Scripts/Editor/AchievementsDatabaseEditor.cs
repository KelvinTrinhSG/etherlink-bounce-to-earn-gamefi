using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Linq;

namespace Watermelon
{
    [CustomEditor(typeof(AchievementsDatabase))]
    public class AchievementsDatabaseEditor : Editor
    {
        private const string PRODUCTS_FOLDER_NAME = "Achievements";

        private const string achievementsPropertyName = "achievements";

        private SerializedProperty achievementsProperty;

        private string folderPath;

        private Type[] allowedTypes;
        private string[] typeNames;

        private int selectedType;

        private SerializedProperty selectedObject;
        private Editor selectedProductEditor;
        private static int selectedObjectInstanceID = -1;

        private AchievementsDatabase achievementsDatabase;

        private void OnEnable()
        {
            achievementsDatabase = (AchievementsDatabase)target;

            //Extract path from database and add folder name
            folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)) + "/" + PRODUCTS_FOLDER_NAME + "/";

            //Check if path folder exist and if not - create it
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //Get properties
            achievementsProperty = serializedObject.FindProperty(achievementsPropertyName);

            //Get store product types
            allowedTypes = Assembly.GetAssembly(typeof(Achievement)).GetTypes().Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(Achievement)) || type.Equals(typeof(Achievement)))).ToArray();
            typeNames = new string[allowedTypes.Length];

            for (int i = 0; i < allowedTypes.Length; i++)
            {
                typeNames[i] = Regex.Replace(allowedTypes[i].ToString(), "([a-z]) ?([A-Z])", "$1 $2");
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            selectedType = EditorGUILayout.Popup("Type:", selectedType, typeNames);
            EditorGUILayout.BeginHorizontal();
            GUI.color = Color.green;
            if (GUILayout.Button("Add", GUILayout.Height(14)))
            {
                GUI.FocusControl(null);

                AddModule(allowedTypes[selectedType]);
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Display objects array box with fixed size
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
            int productsCount = achievementsProperty.arraySize;

            if (productsCount > 0)
            {
                for (int i = 0; i < productsCount; i++)
                {
                    int index = i;
                    SerializedProperty objectProperty = achievementsProperty.GetArrayElementAtIndex(i);

                    if (objectProperty.objectReferenceValue != null)
                    {
                        SerializedObject referenceObject = new SerializedObject(objectProperty.objectReferenceValue);

                        bool isLevelSelected = IsObjectSelected(objectProperty);

                        if (isLevelSelected)
                            GUI.color = Color.green;

                        Rect clickRect = EditorGUILayout.BeginHorizontal(GUI.skin.box);

                        EditorGUILayout.LabelField(objectProperty.objectReferenceValue.name.Replace(".asset", ""));

                        GUILayout.FlexibleSpace();

                        GUI.color = Color.grey;
                        if (GUILayout.Button("=", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
                        {
                            GenericMenu menu = new GenericMenu();

                            int productId = referenceObject.FindProperty("id").intValue;

                            menu.AddItem(new GUIContent("Remove"), false, delegate
                            {
                                if (achievementsProperty.RemoveObject(index))
                                {
                                    UnselectObject();

                                    return;
                                }
                            });

                            menu.AddSeparator("");

                            menu.AddItem(new GUIContent("Source object"), false, delegate
                            {
                                objectProperty.SelectSourceObject();
                            });

                            menu.ShowAsContext();
                        }
                        GUI.color = Color.white;

                        GUILayout.Space(5);

                        if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                        {
                            SelectedObject(objectProperty, i);

                            return;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.HelpBox("Database is empty, add achievement first!", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            if (selectedObject != null && selectedObjectInstanceID != -1)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);

                if (selectedProductEditor == null)
                    Editor.CreateCachedEditor(selectedObject.objectReferenceValue, null, ref selectedProductEditor);

                selectedProductEditor.OnInspectorGUI();

                EditorGUILayout.EndVertical();
            }
        }

        private void UnselectObject()
        {
            GUI.FocusControl(null);
            selectedProductEditor = null;

            selectedObject = null;
            selectedObjectInstanceID = -1;
        }

        private void SelectedObject(SerializedProperty serializedProperty, int index)
        {
            GUI.FocusControl(null);
            selectedProductEditor = null;

            //Check if current selected object is equals to new and unselect it
            if (selectedObject != null && selectedObject.objectReferenceInstanceIDValue == serializedProperty.objectReferenceInstanceIDValue)
            {
                selectedObject = null;
                selectedObjectInstanceID = -1;

                return;
            }

            if (serializedProperty != null)
            {
                selectedObjectInstanceID = serializedProperty.objectReferenceInstanceIDValue;
                selectedObject = serializedProperty;
            }
        }

        private bool IsObjectSelected(SerializedProperty serializedProperty)
        {
            return selectedObject != null && selectedObjectInstanceID == serializedProperty.objectReferenceInstanceIDValue;
        }

        private int GetUniqueProductId()
        {
            if (achievementsDatabase.Achievements != null && achievementsDatabase.Achievements.Length > 0)
            {
                return achievementsDatabase.Achievements.Max(x => x.ID) + 1;
            }
            else
            {
                return 1;
            }
        }

        public void AddModule(Type type)
        {
            if (!type.IsSubclassOf(typeof(Achievement)))
            {
                Debug.LogError("[Achievements]: Achievement type should be subclass of Achievement class!");

                return;
            }

            serializedObject.Update();

            int uniqueID = GetUniqueProductId();

            achievementsProperty.arraySize++;

            Achievement achievement = (Achievement)ScriptableObject.CreateInstance(type);
            achievement.ID = uniqueID;
            achievement.name = type.ToString();
            achievement.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(achievement, target);
            AssetDatabase.SaveAssets();

            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(achievement));
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(target));
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            achievementsProperty.GetArrayElementAtIndex(achievementsProperty.arraySize - 1).objectReferenceValue = achievement;

            serializedObject.ApplyModifiedProperties();
        }
    }
}