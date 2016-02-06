using UnityEngine;
#if UNITY_EDITOR

using UnityEditor;

public class MakePrefabLibrary
{
    [MenuItem("Assets/Create/AnimalBrain")]
    public static void CreateAnimalBrainDataAsset()
    {
        var asset = ScriptableObject.CreateInstance<AnimalBrain>();
        CreateDataAsset("AnimalBrain", asset);
    }

    /*
    [MenuItem("Assets/Create/GUIx")]
    public static void CreateGUIxDataAsset()
    {
        var asset = ScriptableObject.CreateInstance<GUIx>();
        CreateDataAsset("GUIx", asset);
    }
    */

    static void CreateDataAsset(string name, Object asset)
    {
        AssetDatabase.CreateAsset(asset as Object, "Assets/"+ name +".asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset as Object;
    }   
}
#endif
