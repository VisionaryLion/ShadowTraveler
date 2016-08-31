using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Actors
{
    [CustomEditor(typeof(Actor), true)]
    public class ActorEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            GUI.enabled = false;

            SerializedProperty prop = serializedObject.GetIterator();
            if (prop.NextVisible(true))
            {
                do
                {
                    EditorGUILayout.PropertyField(prop);
                } while (prop.NextVisible(false));
            }
            GUI.enabled = true;
            if (GUILayout.Button("Refresh"))
            {
                ((Actor)serializedObject.targetObject).Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
