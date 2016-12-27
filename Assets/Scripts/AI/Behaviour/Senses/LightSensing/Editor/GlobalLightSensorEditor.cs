using UnityEngine;
using UnityEditor;
using System.Collections;
using LightSensing;

[CustomEditor(typeof(GlobalLightSensor))]
public class GlobalLightSensorEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Gather all in scene"))
        {
            serializedObject.Update();
            LightMarker[] allMarkers = GameObject.FindObjectsOfType<LightMarker>();
            SerializedProperty allLightsArray = serializedObject.FindProperty("globalLightMarker");
            allLightsArray.arraySize = allMarkers.Length;
            for (int iMarker = 0; iMarker < allMarkers.Length; iMarker++)
            {
                allLightsArray.GetArrayElementAtIndex(iMarker).objectReferenceValue = allMarkers[iMarker];
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
