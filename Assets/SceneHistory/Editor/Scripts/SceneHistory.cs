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
    const string HistoryListLabel = "HistoryList";
    const string DeleteAllLabel = "DeleteAllHistory";
    const string DeleteAllButtonName = "DeleteAll";
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

        // ----- 現在の履歴シーン一覧 -----
        EditorGUILayout.LabelField(HistoryListLabel, EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
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
        EditorGUI.indentLevel--;
        // 区切り線
        EditorGUILayout.Space();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        EditorGUILayout.Space();


        //  ----- 全履歴削除ボタン -----
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField(DeleteAllLabel, EditorStyles.boldLabel);
        EditorGUILayout.Space();
        EditorGUI.indentLevel++;
        // ボタンの背景色を赤色に
        GUI.backgroundColor = Color.red;
        if (GUILayout.Button(DeleteAllButtonName))
        {
            datas.Clear();
        }
        GUI.backgroundColor = Color.white;
        EditorGUI.indentLevel--;
        // 区切り線
        EditorGUILayout.Space();
        GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
        EditorGUILayout.Space();


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