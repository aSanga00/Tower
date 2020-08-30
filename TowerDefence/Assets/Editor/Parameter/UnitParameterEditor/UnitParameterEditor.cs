using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Battle.Unit;

public class UnitParameterEditor : EditorWindow
{

    private const string BASE_PATH = "Assets/Resources/Data/";

    private const string ENEMY_PATH = "EnemyParameter/";

    private const string Unit_PATH = "UnitParameter/";

    private const string ASSET_PATH = "Assets/Resources/Data/EnemyParameter/EnemyData01.asset";

    private const string TEST_PATH = "Data/EnemyParameter/EnemyData01";

    private BaseParameter parameterObject;

    [MenuItem("Editor/Parameter/Chara")]
    private static void Create()
    {
        GetWindow<UnitParameterEditor>("キャラパラメーターエディター");
    }

    private void OnGUI()
    {
        if(parameterObject == null)
        {
            Load();
        }

        using (new GUILayout.HorizontalScope())
        {
            using (new GUILayout.VerticalScope())
            {
                parameterObject.id = EditorGUILayout.IntField("ID", parameterObject.id);
                parameterObject.name = EditorGUILayout.TextField("NAME", parameterObject.name);
                parameterObject.hp = EditorGUILayout.IntField("HP", parameterObject.hp);
                parameterObject.cost = EditorGUILayout.IntField("COST", parameterObject.cost);
                parameterObject.attack = EditorGUILayout.IntField("ATK", parameterObject.attack);
                parameterObject.defence = EditorGUILayout.IntField("DEF", parameterObject.defence);
                parameterObject.speed = EditorGUILayout.FloatField("SPD", parameterObject.speed);
                parameterObject.searchRange = EditorGUILayout.FloatField("SRange", parameterObject.searchRange);
                parameterObject.attackRange = EditorGUILayout.FloatField("AtkRange", parameterObject.attackRange);
                parameterObject.coolTime = EditorGUILayout.FloatField("CTime", parameterObject.coolTime);
            }
        }

        using (new GUILayout.HorizontalScope())
        {
            //読み込みボタン
            if (GUILayout.Button("読み込み"))
            {
                Load();
            }
        }

        using (new GUILayout.HorizontalScope())
        {
            //書き込みボタン
            if (GUILayout.Button("書き込み"))
            {
                Export();
            }
        }
    }

    private void Load()
    {
        parameterObject = Resources.Load<BaseParameter>(TEST_PATH);

        if(parameterObject == null)
        {
            parameterObject = new BaseParameter();
        }

        if (!AssetDatabase.Contains(parameterObject as UnityEngine.Object))
        {
            string directory = Path.GetDirectoryName(ASSET_PATH);

            //アセット作成
            AssetDatabase.CreateAsset(parameterObject, ASSET_PATH);
        }

        //インスペクターから設定できないようにする
        parameterObject.hideFlags = HideFlags.NotEditable;

        //更新通知
        EditorUtility.SetDirty(parameterObject);

        //保存
        AssetDatabase.SaveAssets();

        //エディタを最新の状態にする
        AssetDatabase.Refresh();
    }

    private void Export()
    {
        if(!AssetDatabase.Contains(parameterObject as UnityEngine.Object))
        {
            string directory = Path.GetDirectoryName(ASSET_PATH);

            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //アセット作成
            AssetDatabase.CreateAsset(parameterObject, ASSET_PATH);
        }

        //インスペクターから設定できないようにする
        parameterObject.hideFlags = HideFlags.NotEditable;

        //更新通知
        EditorUtility.SetDirty(parameterObject);

        //保存
        AssetDatabase.SaveAssets();

        //エディタを最新の状態にする
        AssetDatabase.Refresh();
    }
}
