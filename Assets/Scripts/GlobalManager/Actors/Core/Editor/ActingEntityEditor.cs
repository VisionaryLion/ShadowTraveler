using UnityEngine;
using System.Collections;
using UnityEditor;

public class ActingEntityEditor : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

namespace Entities
{
    [CustomEditor(typeof(ActingEntity), true)]
    [CanEditMultipleObjects]
    public class ActingEntityEditor : Editor
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
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh"))
            {
                foreach (var t in targets)
                    ((ActingEntity)t).Refresh();
            }
            if (GUILayout.Button("Change Name"))
            {
                foreach (var t in targets)
                    ((ActingEntity)t).Rename();
            }
            EditorGUILayout.EndHorizontal();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
