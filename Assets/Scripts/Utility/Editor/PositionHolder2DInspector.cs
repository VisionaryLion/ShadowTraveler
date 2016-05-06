using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PositionHolder2D))]
[CanEditMultipleObjects]
public class PositionHolder2DInspector : Editor
{

    SerializedProperty positions;
    int currentSelectedPosition = 0;
    bool shouldConnectEnds;
    bool showPositions;

    void OnEnable()
    {
        positions = serializedObject.FindProperty("positions");
        if (positions.arraySize < 1)
        {
            positions.arraySize++;
            positions.serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }

    public override void OnInspectorGUI()
    {
        // Update the serializedProperty
        serializedObject.Update();
       
        showPositions = EditorGUILayout.Foldout(showPositions, "Positions");
        EditorGUI.BeginChangeCheck();
        if (showPositions)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < positions.arraySize; i++)
            {
                EditorGUILayout.PropertyField(positions.GetArrayElementAtIndex(i), new GUIContent("Position " + i));
                if (i == currentSelectedPosition)
                {
                    EditorGUI.DrawRect(GUILayoutUtility.GetLastRect(), new Color(0, 0, 1, 0.1f));
                }
            }
            EditorGUI.indentLevel--;
            
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Rect positionContainer = GUILayoutUtility.GetLastRect();
                positionContainer.yMin -= (positionContainer.height + EditorGUIUtility.standardVerticalSpacing) * (positions.arraySize - 1);
                if (positionContainer.Contains(Event.current.mousePosition))
                {
                    currentSelectedPosition = Mathf.FloorToInt((Event.current.mousePosition.y - positionContainer.yMin) / (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight));
                    Repaint();
                }
            }
        }
        shouldConnectEnds = EditorGUILayout.Toggle("Connect Last, First", shouldConnectEnds);
        Rect buttonRect = GUILayoutUtility.GetLastRect();
        buttonRect.y += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
        buttonRect.width = (buttonRect.width - 20) / 2;
        GUI.enabled = showPositions;
        if (GUI.Button(buttonRect, "Insert"))
        {
            positions.InsertArrayElementAtIndex(currentSelectedPosition);
            currentSelectedPosition++;
            Repaint();
        }
        buttonRect.x = buttonRect.width + 20;
        if (GUI.Button(buttonRect, "-"))
            RemoveArrayElementAt(positions, currentSelectedPosition);
        GUILayout.Space(EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight);
        GUI.enabled = true;
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            SceneView.RepaintAll();
        }
    }

    public void OnSceneGUI()
    {
        Vector3 currentPos;
        Vector3 oldPos = positions.GetArrayElementAtIndex(0).vector2Value;
        bool didChangeHappen = false;
        Handles.Label(oldPos, "Position 0");
        Vector3 newPos = Handles.PositionHandle(oldPos, Quaternion.identity);
        newPos.z = 0;
        if (newPos != oldPos)
        {
            didChangeHappen = true;
            positions.GetArrayElementAtIndex(0).vector2Value = newPos;
        }
        oldPos = newPos;
        for (int i = 1; i < positions.arraySize; i++)
        {
            currentPos = positions.GetArrayElementAtIndex(i).vector2Value;
            Handles.Label(currentPos, "Position "+i);
            Handles.DrawLine(oldPos, currentPos);
            newPos = Handles.PositionHandle(currentPos, Quaternion.identity);
            newPos.z = 0;
            if (newPos != currentPos)
            {
                didChangeHappen = true;
                positions.GetArrayElementAtIndex(i).vector2Value = newPos;
            }
            oldPos = newPos;
        }
        if(shouldConnectEnds)
            Handles.DrawLine(oldPos, positions.GetArrayElementAtIndex(0).vector2Value);
        if (didChangeHappen)
            positions.serializedObject.ApplyModifiedProperties();
    }

    void RemoveArrayElementAt (SerializedProperty array, int index)
    {
        Vector2[] buffer = new Vector2[array.arraySize - 1];
        for (int i = 0; i < index; i++)
        {
            buffer[i] = array.GetArrayElementAtIndex(i).vector2Value;
        }
        for (int i = index + 1; i < array.arraySize; i++)
        {
            buffer[i -1] = array.GetArrayElementAtIndex(i).vector2Value;
        }
        array.arraySize--;
        for (int i = 0; i < array.arraySize; i++)
        {
            array.GetArrayElementAtIndex(i).vector2Value = buffer[i];
        }
        serializedObject.ApplyModifiedProperties();
        SceneView.RepaintAll();
    }
}
