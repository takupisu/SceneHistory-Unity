#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
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
    Vector2 scrollPosition = new Vector2(0, 0);

    const string DeleteAllButtonName = "DeleteAll";
    const string LoadButtonLabel = "Load";
    const string AdditiveToggleLabel = "Add";
    const string DeletebButtonLabel = "Del";
    const float ElementHeight = 20f;
    const float ElementHeightSpace = 5f;
    const float ElementWidthSpace = 20f;
    const float LoadButtonLabelX = 12f;
    const float AdditiveToggleWidth = 15f;
    const float AdditiveToggleLabelWidth = 25f;
    const float AdditiveToggleY = 2f;
    const float DeleteButtonLabelX = 9f;
    const float DeleteButtonWidth = 30f;
    const float AllDeleteButtonHeight = 20f;
    const float AllDeleteButtonWidth = 90f;
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
        if( null != datas.Find(d => d.path == scene.path))
        {
            return;
        }
        SceneHistoryData data = new SceneHistoryData(scene.name, scene.path, mode);
        datas.Insert(0,data);
        Repaint();
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
        reorderableList.DoLayoutList();

        /*
        //  ----- 全履歴削除ボタン -----
        Rect rect = GUILayoutUtility.GetLastRect();
        rect.width = AllDeleteButtonWidth;
        // ボタンの背景色を赤色に
        GUI.backgroundColor = Color.red;
        if (GUI.Button(rect,DeleteAllButtonName))
        {
            DeleteAllHistoryData();
        }
        GUI.backgroundColor = Color.white;
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
        loadButtonRect.x += LoadButtonLabelX;
        loadButtonRect.width = rect.width - (ElementWidthSpace + AdditiveToggleWidth + ElementWidthSpace + DeleteButtonWidth);
        EditorGUI.LabelField(loadButtonRect, LoadButtonLabel);
        loadButtonRect.x -= LoadButtonLabelX;

        // 追加チェックボックス
        Rect additiveToggleRect = new Rect(loadButtonRect);
        additiveToggleRect.x += loadButtonRect.width + ElementWidthSpace;
        additiveToggleRect.width = AdditiveToggleLabelWidth;
        EditorGUI.LabelField(additiveToggleRect, AdditiveToggleLabel);

        // 削除ボタン
        Rect deleteButtonRect = new Rect(additiveToggleRect);
        deleteButtonRect.x += additiveToggleRect.width + ElementWidthSpace - DeleteButtonLabelX;
        deleteButtonRect.width = DeleteButtonWidth;
        EditorGUI.LabelField(deleteButtonRect, DeletebButtonLabel);
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
            // 必要なら保存確認ダイアログ表示
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(datas[index].path, datas[index].mode);
            }
        }

        // 追加チェックボックス
        Rect additiveToggleRect = new Rect(loadButtonRect);
        additiveToggleRect.x += loadButtonRect.width + ElementWidthSpace;
        additiveToggleRect.y += AdditiveToggleY;
        additiveToggleRect.width = AdditiveToggleWidth;
        datas[index].mode = GUI.Toggle(additiveToggleRect, datas[index].mode == OpenSceneMode.Additive, GUIContent.none) ? OpenSceneMode.Additive : OpenSceneMode.Single;
        additiveToggleRect.y -= AdditiveToggleY;

        // 削除ボタン
        Rect deleteButtonRect = new Rect(additiveToggleRect);
        deleteButtonRect.x += additiveToggleRect.width + ElementWidthSpace;
        deleteButtonRect.width = DeleteButtonWidth;
        if (GUI.Button(deleteButtonRect, trashTexture, EditorStyles.label))
        {
            datas.RemoveAt(index);
        }
    }

    /// <summary>
    /// 全履歴削除
    /// </summary>
    void DeleteAllHistoryData()
    {
        datas.Clear();
    }

    /// <summary>
    /// 履歴データ
    /// </summary>
    [Serializable]
    public class SceneHistoryData
    {
        public SceneHistoryData(string n, string p, OpenSceneMode m)
        {
            name = n;
            path = p;
            mode = m;
        }

        /// <summary>
        /// シーン名
        /// </summary>
        public string name;

        /// <summary>
        /// シーンのファイル
        /// </summary>
        public string path;

        /// <summary>
        /// シーンの読み込みモード
        /// </summary>
        public OpenSceneMode mode;
    }
}
#endif