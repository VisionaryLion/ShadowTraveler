using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Entity
{
    [CustomEditor(typeof(Entity), true)]
    public class EntityEditor : Editor
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
                ((Entity)serializedObject.targetObject).Refresh();
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
