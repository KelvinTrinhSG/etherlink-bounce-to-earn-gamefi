using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.Serialization.Formatters.Binary;

namespace Watermelon
{
    [CustomEditor(typeof(StoreSettings))]
    public class StoreDataEditor : Editor
    {
        private const string PRODUCTS_FOLDER_NAME = "Products";

        private const string productsPropertyName = "products";

        private SerializedProperty productsProperty;
        
        private string folderPath;

        private Type[] allowedTypes;
        private string[] typeNames;

        private int selectedType;

        private string selectedObjectName;

        private SerializedProperty selectedObject;
        private Editor selectedProductEditor;
        private static int selectedObjectInstanceID = -1;

        private void OnEnable()
        {
            //Extract path from database and add folder name
            folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)) + "/" + PRODUCTS_FOLDER_NAME + "/";

            //Check if path folder exist and if not - create it
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            //Get properties
            productsProperty = serializedObject.FindProperty(productsPropertyName);
            
            //Get store product types
            allowedTypes = Assembly.GetAssembly(typeof(StoreProduct)).GetTypes().Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(StoreProduct)) || type.Equals(typeof(StoreProduct)))).ToArray();
            typeNames = new string[allowedTypes.Length];

            for (int i = 0; i < allowedTypes.Length; i++)
            {
                typeNames[i] = Regex.Replace(allowedTypes[i].ToString(), "([a-z]) ?([A-Z])", "$1 $2");
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            selectedType = EditorGUILayout.Popup("Type:", selectedType, typeNames);
            EditorGUILayout.BeginHorizontal();
            selectedObjectName = EditorGUILayout.TextField("Object Name:", selectedObjectName);
            GUI.color = EditorColor.green04;
            if (GUILayout.Button("Add", GUILayout.Height(14)))
            {
                if (!string.IsNullOrEmpty(selectedObjectName))
                {
                    GUI.FocusControl(null);

                    StoreProduct product = CreateProduct(allowedTypes[selectedType], selectedObjectName);
                    if (productsProperty.AddObject(product))
                    {
                        product.ID = productsProperty.arraySize;

                        selectedObjectName = "";

                        return;
                    }
                }
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Display objects array box with fixed size
            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.ExpandWidth(true));
            int productsCount = productsProperty.arraySize;
            for (int i = 0; i < productsCount; i++)
            {
                int index = i;
                SerializedProperty objectProperty = productsProperty.GetArrayElementAtIndex(i);

                if (objectProperty.objectReferenceValue != null)
                {
                    SerializedObject referenceObject = new SerializedObject(objectProperty.objectReferenceValue);
                    
                    bool isLevelSelected = IsObjectSelected(objectProperty);

                    if (isLevelSelected)
                        GUI.color = EditorColor.green05;

                    Rect clickRect = EditorGUILayout.BeginHorizontal(GUI.skin.box);

                    string title = referenceObject.FindProperty("productName").stringValue;
                    EditorGUILayout.LabelField(string.IsNullOrEmpty(title) ? objectProperty.objectReferenceValue.name.Replace(".asset", "") : title);

                    GUILayout.FlexibleSpace();

                    GUI.color = Color.grey;
                    if (GUILayout.Button("=", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        GenericMenu menu = new GenericMenu();

                        int productId = referenceObject.FindProperty("id").intValue;

                        menu.AddItem(new GUIContent("Remove"), false, delegate
                        {
                            if (productsProperty.RemoveObject(index))
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

                        menu.AddSeparator("");

                        menu.AddItem(new GUIContent("Unlock"), false, delegate
                        {
                            UnlockProduct(productId);
                        });

                        menu.AddItem(new GUIContent("Lock"), false, delegate
                        {
                            LockProduct(productId);
                        });

                        menu.AddSeparator("");

                        if (i > 0)
                        {
                            menu.AddItem(new GUIContent("Move up"), false, delegate
                            {
                                productsProperty.MoveArrayElement(index, index - 1);
                                serializedObject.ApplyModifiedProperties();

                                if (selectedObject != null)
                                    UnselectObject();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Move up"));
                        }


                        if (i + 1 < productsCount)
                        {
                            menu.AddItem(new GUIContent("Move down"), false, delegate
                            {
                                productsProperty.MoveArrayElement(index, index + 1);
                                serializedObject.ApplyModifiedProperties();

                                if (selectedObject != null)
                                    UnselectObject();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Move down"));
                        }

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

                    if (selectedObject != null && selectedObjectInstanceID != -1 && selectedObjectInstanceID == objectProperty.objectReferenceInstanceIDValue)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);

                        if (selectedProductEditor == null)
                            Editor.CreateCachedEditor(selectedObject.objectReferenceValue, null, ref selectedProductEditor);

                        selectedProductEditor.OnInspectorGUI();

                        EditorGUILayout.EndVertical();
                    }
                }
            }

            if (productsCount == 0)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.HelpBox("Tab is empty, add products first!", MessageType.Warning);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
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

        private void ClearProducts()
        {
            StoreController.SaveData storeData = LoadData();

            storeData.ClearItems();

            SaveData(storeData);
        }

        private void UnlockProduct(int id)
        {
            StoreController.SaveData storeData = LoadData();

            storeData.AddBoughtItem(id);

            SaveData(storeData);
        }

        private void LockProduct(int id)
        {
            StoreController.SaveData storeData = LoadData();

            storeData.RemoveItem(id);

            SaveData(storeData);
        }

        private void SaveData(StoreController.SaveData storeData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + StoreController.FILE_NAME, FileMode.Create);

            bf.Serialize(file, storeData);

            file.Close();
        }

        private StoreController.SaveData LoadData()
        {
            if (File.Exists(Application.persistentDataPath + "/" + StoreController.FILE_NAME))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/" + StoreController.FILE_NAME, FileMode.Open);

                StoreController.SaveData saveData = (StoreController.SaveData)bf.Deserialize(file);

                file.Close();

                return saveData;
            }

            return new StoreController.SaveData();
        }

        private bool IsObjectSelected(SerializedProperty serializedProperty)
        {
            return selectedObject != null && selectedObjectInstanceID == serializedProperty.objectReferenceInstanceIDValue;
        }

        private StoreProduct CreateProduct(Type type, string name)
        {
            return EditorUtils.CreateAsset<StoreProduct>(type, folderPath + name.Replace(" ", "") + "Product" + (productsProperty.arraySize + 1).ToString("000") + ".asset", true);
        }

        [InitializeOnLoadMethod]
        public static void StoreImporter()
        {
            IOUtils.CreatePath(ApplicationConsts.PROJECT_FOLDER + "/Content/Store/" + PRODUCTS_FOLDER_NAME + "/");
        }
    }
}