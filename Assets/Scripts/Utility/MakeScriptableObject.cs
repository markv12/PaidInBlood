#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

public class MakeScriptableObject
{
    [MenuItem("Assets/Create/It")]
    public static void CreateMyAsset()
    {
        StartingDeck asset = ScriptableObject.CreateInstance<StartingDeck>();

        AssetDatabase.CreateAsset(asset, "Assets/StartingDeck.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
}
#endif