using UnityEngine;
using System.Collections;
using UnityEditor;

public class LightSkinAsset {

    [MenuItem("Pathfinding/NavAgentSettings/LightSkin")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<LightSkin>("LightSkin");
    }
}
