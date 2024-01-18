using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateAtomData
{
    [MenuItem("Assets/Create/Atom Data")]
    public static void MakeAtomData()
    {

    string name = "AtomData";
        AtomData asset = ScriptableObject.CreateInstance<AtomData>();  
        AssetDatabase.CreateAsset(asset, $"Assets/ScriptableObjects/{name}.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
