using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using Watermelon;

public class PresetEditorWindow : EditorWindow
{
    private string SCENES_PATH
    {
        get { return "/" + ApplicationConsts.PROJECT_FOLDER + "/Game/Scenes/PresetSetuper.unity"; }
    }

    private ColorsPreset selectedPreset;
    private Editor selectedPresetEditor;

    private bool isPresetSetuperScene;

    private Vector2 scrollPosition;

    private const string SCENE_NAME = "PresetSetuper";

    [MenuItem("Tools/Editor/Preset Editor")]
    public static void Init()
    {
        GetWindow(typeof(PresetEditorWindow), false, "Preset Editor");
    }

    private void OnEnable()
    {
        if (selectedPreset != null)
            selectedPresetEditor = Editor.CreateEditor(selectedPreset);

        if (!File.Exists(Application.dataPath + SCENES_PATH))
        {
            Debug.LogWarning("Preset Setuper scene doesn't exist! Create it first!");
        }

        isPresetSetuperScene = EditorSceneManager.GetSceneByName("PresetSetuper").name == SCENE_NAME;

        EditorApplication.hierarchyChanged += OnHierarchyWindowChanged;
        EditorApplication.playModeStateChanged += PlayModeStateChanged;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= OnHierarchyWindowChanged;
        EditorApplication.playModeStateChanged -= PlayModeStateChanged;
    }

    private void PlayModeStateChanged(PlayModeStateChange playModeState)
    {
        isPresetSetuperScene = EditorSceneManager.GetSceneByName("PresetSetuper").name == SCENE_NAME;
    }

    private void OnHierarchyWindowChanged()
    {
        isPresetSetuperScene = EditorSceneManager.GetSceneByName("PresetSetuper").name == SCENE_NAME;
    }

    private void OnGUI()
    {
        if (isPresetSetuperScene)
        {
            if(Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUI.BeginChangeCheck();
                selectedPreset = (ColorsPreset)EditorGUILayout.ObjectField(new GUIContent("Color Preset"), selectedPreset, typeof(ColorsPreset), false);
                if (EditorGUI.EndChangeCheck())
                {
                    selectedPresetEditor = selectedPreset != null ? Editor.CreateEditor(selectedPreset) : null;
                }
                EditorGUILayout.EndHorizontal();

                if (selectedPreset != null)
                {
                    if (selectedPresetEditor != null)
                    {
                        EditorGUILayout.BeginVertical(GUI.skin.box);
                        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                        selectedPresetEditor.OnInspectorGUI();
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Something went wrong :( \nReopen editor window!", MessageType.Warning);
                    }
                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Preview", GUILayout.Height(40f)))
                    {
                        ColorsController.SetPreset(selectedPreset);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Please, select color preset first!", MessageType.Info);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("Start game to use Preset Editor!", MessageType.Warning);
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Open Preset Setuper scene to be able to use Preset Editor!", MessageType.Warning);

            if (GUILayout.Button("Open Scene"))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene("Assets" + SCENES_PATH, OpenSceneMode.Single);
                }
            }
        }
    }
}
