#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Sceneの読み込み履歴を保持し、履歴からSceneを読み込める機能
/// </summary>
[InitializeOnLoad]
public class SceneHistory : EditorWindow
{
    static List<SceneHistoryData> datas = new List<SceneHistoryData>();
    //const string PrefsKey = "SceneHistory";
    Vector2 scrollPosition = new Vector2(0, 0);

    static SceneHistory()
    {
        EditorSceneManager.sceneOpened += OnOpened;
    }

    static void OnOpened(Scene scene, OpenSceneMode mode)
    {
        SceneHistoryData data = new SceneHistoryData(scene.name, scene.path, mode);
        if (datas.Contains(data))
        {
            datas.Remove(data);
        }
        datas.Insert(0,data);

        /*
        string str = EditorPrefs.GetString(PrefsKey);
        str += ":" + scene.path;
        EditorPrefs.SetString(PrefsKey, str);
        */
    }

    [MenuItem("Window/SceneHistory")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<SceneHistory>();
    }

    void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        for (int i = 0; i < datas.Count; i++)
        {
            if (GUILayout.Button(datas[i].name))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(datas[i].path, datas[i].mode);
                }
            }
        }
        EditorGUILayout.EndScrollView();
    }

    struct SceneHistoryData
    {
        public SceneHistoryData(string n, string p, OpenSceneMode m)
        {
            name = n;
            path = p;
            mode = m;
        }
        public string name;
        public string path;
        public OpenSceneMode mode;
    }
}
#endif