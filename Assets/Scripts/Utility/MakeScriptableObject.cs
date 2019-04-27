#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/It")]
    public static void CreateMyAsset()
    {
        CardData asset = ScriptableObject.CreateInstance<CardData>();

        AssetDatabase.CreateAsset(asset, "Assets/CardEffect.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif