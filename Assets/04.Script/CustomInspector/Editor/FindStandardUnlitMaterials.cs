using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FindStandardUnlitMaterials
{
    [MenuItem("Tools/Find Particles/Standard Unlit Materials")]
    static void FindMaterials()
    {
        var shaderName = "Particles/Standard Unlit";
        var guids = AssetDatabase.FindAssets("t:Material");
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && mat.shader != null && mat.shader.name == shaderName)
            {
                Debug.Log($"Particles/Standard Unlit 사용: {path}", mat);
            }
        }
    }
}