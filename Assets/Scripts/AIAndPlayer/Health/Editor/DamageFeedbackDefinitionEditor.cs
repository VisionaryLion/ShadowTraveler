using UnityEditor;
using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    [CustomEditor(typeof(DamageFeedbackDefinition))]
    public class DamageFeedbackDefinitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DamageFeedbackDefinition myTarget = (DamageFeedbackDefinition)target;
            myTarget.ResizeOrCreateAudioClips();
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < IDamageInfo.DamageTypCount; i++)
            {
                SerializedProperty tps = serializedObject.FindProperty("audioClips").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(tps, new GUIContent(IDamageInfo.DamageTypToString(i)), true);
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
