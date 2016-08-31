using UnityEditor;
using UnityEngine;

/*
Author: Oribow
*/
namespace Combat
{
    [CustomPropertyDrawer(typeof(Resistance))]
    public class ResistanceEditor : PropertyDrawer
    {
        public bool foldout = true;
        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            label.text += " multiplicator";
            foldout = EditorGUI.Foldout(position, foldout, label);
            if (foldout)
            {
                // Don't make child fields be indented
                int indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = indent+1;
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                // Draw fields - passs GUIContent.none to each so they are drawn without labels
                SerializedProperty resArray = property.FindPropertyRelative("resistances");
                if (resArray.arraySize < IDamageInfo.DamageTypCount)
                {
                    int oldSize = resArray.arraySize;
                    resArray.arraySize = IDamageInfo.DamageTypCount;
                    do
                    {
                        resArray.InsertArrayElementAtIndex(oldSize++);
                        resArray.GetArrayElementAtIndex(oldSize - 1).floatValue = 1;
                    } while (oldSize != IDamageInfo.DamageTypCount);
                    property.serializedObject.ApplyModifiedProperties();
                }
                else if (resArray.arraySize > IDamageInfo.DamageTypCount)
                {
                    resArray.arraySize = IDamageInfo.DamageTypCount;
                    property.serializedObject.ApplyModifiedProperties();
                }
                position.height = EditorGUIUtility.singleLineHeight;
                for (int i = 0; i < resArray.arraySize; i++)
                {
                    EditorGUI.PropertyField(position, resArray.GetArrayElementAtIndex(i), new GUIContent(IDamageInfo.DamageTypToString(i)));
                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                }
                // Set indent back to what it was
                EditorGUI.indentLevel = indent;
            }
            

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {

            if (foldout)
            {
                property = property.FindPropertyRelative("resistances");
                return (property.arraySize + 1) * EditorGUIUtility.singleLineHeight + (property.arraySize) * EditorGUIUtility.standardVerticalSpacing;
            }
            else
            {
                return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            }
        }
    }
}
