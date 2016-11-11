using UnityEngine;
using UnityEditor;

public class ObjectiveDataAsset
{
    [MenuItem("Assets/Create/Objective Data")]
    public static void CreateAsset ()
    {
        ScriptableObjectUtility.CreateAsset<ObjectiveData>("ObjectiveData");
    }

}
