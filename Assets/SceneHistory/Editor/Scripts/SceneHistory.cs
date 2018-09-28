#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using System;

/// <summary>
/// Sceneの読み込み履歴を保持し、履歴からSceneを読み込める機能
/// </summary>
[InitializeOnLoad]
public class SceneHistory : EditorWindow
{
    ReorderableList reorderableList;
    [SerializeField]
    List<SceneHistoryData> datas = new List<SceneHistoryData>();
    //const string PrefsKey = "SceneHistory";
    const string HistoryListLabel = "HistoryList";
    const string DeleteAllLabel = "DeleteAllHistory";
    const string DeleteAllButtonName = "DeleteAll";
    Vector2 scrollPosition = new Vector2(0, 0);
    const float ElementHeight = 20f;
    const float ElementHeightSpace = 5f;
    const float ElementWidthSpace = 20f;
    const float AdditiveToggleWidth = 15f;
    const float DeleteButtonWidth = 30f;
    Texture trashTexture;

    private void OnEnable()
    {
        EditorSceneManager.sceneOpened += OnOpened;

        trashTexture = EditorGUIUtility.Load("icons/d_treeeditor.trash.png") as Texture2D;

        reorderableList = new ReorderableList(datas, typeof(SceneHistoryData));
        reorderableList.elementHeight = ElementHeight + ElementHeightSpace;
        reorderableList.drawElementCallback += DrawElement;
        reorderableList.drawHeaderCallback += DrawHeader;
        reorderableList.drawFooterCallback += DrawFooter;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnOpened;
    }

    void OnOpened(Scene scene, OpenSceneMode mode)
    {
        SceneHistoryData data = new SceneHistoryData(scene.name, scene.path, mode);
        datas.Remove(datas.Find(d => d.path == scene.path));
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

        reorderableList.DoLayoutList();
        /*
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
        */

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 履歴リストのヘッダー描画
    /// </summary>
    private void DrawHeader(Rect rect)
    {
        // 読み込みボタン
        Rect loadButtonRect = new Rect(rect);
        loadButtonRect.width = rect.width - (ElementWidthSpace + AdditiveToggleWidth + ElementWidthSpace + DeleteButtonWidth);
        EditorGUI.LabelField(loadButtonRect, "読み込み");

        // 追加チェックボックス
        Rect additiveToggleRect = new Rect(loadButtonRect);
        additiveToggleRect.x += loadButtonRect.width + ElementWidthSpace;
        additiveToggleRect.width = AdditiveToggleWidth;
        EditorGUI.LabelField(additiveToggleRect, "追加設定");

        // 削除ボタン
        Rect deleteButtonRect = new Rect(additiveToggleRect);
        deleteButtonRect.x += additiveToggleRect.width + ElementWidthSpace;
        deleteButtonRect.width = DeleteButtonWidth;
        EditorGUI.LabelField(deleteButtonRect, "削除");
    }

    /// <summary>
    /// 履歴リストのフッター描画
    /// </summary>
    private void DrawFooter(Rect rect)
    {
        // nothing
    }

    /// <summary>
    /// 履歴リストの各要素（読み込みボタン、追加設定チェックボックス、削除ボタン）描画
    /// </summary>
    private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        if(datas.Count <= index){
            return;
        }

        // 要素の縦サイズ
        rect.height = ElementHeight;

        // 読み込みボタン
        Rect loadButtonRect = new Rect(rect);
        loadButtonRect.width = rect.width - (ElementWidthSpace + AdditiveToggleWidth + ElementWidthSpace + DeleteButtonWidth);
        if (GUI.Button(loadButtonRect,datas[index].name))
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(datas[index].path, datas[index].mode);
            }
        }

        // 追加チェックボックス
        Rect additiveToggleRect = new Rect(loadButtonRect);
        additiveToggleRect.x += loadButtonRect.width + ElementWidthSpace;
        additiveToggleRect.y += 2f;
        additiveToggleRect.width = AdditiveToggleWidth;
        datas[index].mode = GUI.Toggle(additiveToggleRect, datas[index].mode == OpenSceneMode.Additive, GUIContent.none) ? OpenSceneMode.Additive : OpenSceneMode.Single;
        additiveToggleRect.y -= 2f;

        // 削除ボタン
        Rect deleteButtonRect = new Rect(additiveToggleRect);
        deleteButtonRect.x += additiveToggleRect.width + ElementWidthSpace;
        deleteButtonRect.width = DeleteButtonWidth;
        if (GUI.Button(deleteButtonRect, trashTexture, EditorStyles.label))
        {
            datas.RemoveAt(index);
        }
      
    }

    [Serializable]
    public class SceneHistoryData
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