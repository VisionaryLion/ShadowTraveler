using UnityEngine;
using System.Collections;
using UnityEditor;

public class InteractiveInputDefinitionAsset {

    [MenuItem("Assets/Create/Input/InteractiveInputDef")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<InteractiveInputDefinition>("InteractiveInputDefinition");
    }
}
