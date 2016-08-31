using UnityEditor;
using UnityEngine;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    [CustomEditor(typeof(FootstepsDefinition))]
    public class FootstepsDefinitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            FootstepsDefinition myTarget = (FootstepsDefinition)target;
            myTarget.ResizeOrCreateAudioClips();
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < SurfaceTypes.SurfaceTypeCount; i++)
            {
                SerializedProperty tps = serializedObject.FindProperty("audioClips").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(tps, new GUIContent(SurfaceTypes.SurfaceTypeToString((SurfaceTypes.SurfaceType)i)), true);
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
