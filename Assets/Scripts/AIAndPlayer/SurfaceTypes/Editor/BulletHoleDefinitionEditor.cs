using UnityEditor;
using UnityEngine;

/*
Author: Oribow
*/
namespace SurfaceTypeUser
{
    [CustomEditor(typeof(BulletHoleDefinition))]
    public class BulletHoleDefinitionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BulletHoleDefinition myTarget = (BulletHoleDefinition)target;
            myTarget.ResizeOrCreateSprites();
            EditorGUI.BeginChangeCheck();
            for (int i = 0; i < SurfaceTypes.SurfaceTypeCount; i++)
            {
                SerializedProperty tps = serializedObject.FindProperty("sprites").GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(tps, new GUIContent(SurfaceTypes.SurfaceTypeToString((SurfaceTypes.SurfaceType)i)), true);
            }
            if (EditorGUI.EndChangeCheck())
                serializedObject.ApplyModifiedProperties();
        }
    }
}
