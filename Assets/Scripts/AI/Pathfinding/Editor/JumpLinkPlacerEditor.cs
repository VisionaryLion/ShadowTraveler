using UnityEngine;
using System.Collections;
using UnityEditor;
using NavMesh2D.Core;

namespace Pathfinding2D
{
    [CustomEditor(typeof(JumpLinkPlacer))]
    public class JumpLinkPlacerEditor : Editor
    {
        /*
        bool mappedVarsFoldout = true;
        JumpLinkPlacer jumpLinkPlacer;
        SerializedProperty navPointA;
        SerializedProperty navPointB;
        SerializedProperty nodeIdA;
        SerializedProperty nodeIdB;
        SerializedProperty spField;
        SerializedProperty spNavAgentSettings;
        SerializedProperty worldPointB;
        SerializedProperty spJumpArc;

        NavAgentGroundWalkerSettings navAgentSettings;
        ExpandedTree field;
        Vector3 oldPos;


        //jumparc stuff
        bool jumpLinkValid;
        float arcTargetJ;
        float arcLowerBound;
        float arcUpperBound;

        void OnEnable()
        {
            Tools.hidden = true;
            jumpLinkPlacer = (JumpLinkPlacer)target;
            navPointA = serializedObject.FindProperty("navPointA");
            navPointB = serializedObject.FindProperty("navPointB");
            nodeIdA = serializedObject.FindProperty("nodeIdA");
            nodeIdB = serializedObject.FindProperty("nodeIdB");
            spField = serializedObject.FindProperty("field");
            spNavAgentSettings = serializedObject.FindProperty("navAgentSettings");
            worldPointB = serializedObject.FindProperty("worldPointB");
            spJumpArc = serializedObject.FindProperty("jumpArc");
            navAgentSettings = (NavAgentGroundWalkerSettings)spNavAgentSettings.objectReferenceValue;
            field = (ExpandedTree)spField.objectReferenceValue;

            //spJumpArcMinX = spJumpArc.FindPropertyRelative("minX");
            //spJumpArcMaxX = spJumpArc.FindPropertyRelative("maxX");
        }

        void OnDisable()
        {
            Tools.hidden = false;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            navAgentSettings = (NavAgentGroundWalkerSettings)EditorGUILayout.ObjectField("Ground Walker Settings", navAgentSettings, typeof(NavAgentGroundWalkerSettings), false);
            if (navAgentSettings == null)
                EditorGUILayout.HelpBox("Assign a valid set of settings", MessageType.Error);
            spNavAgentSettings.objectReferenceValue = navAgentSettings;

            field = (ExpandedTree)EditorGUILayout.ObjectField("Expanded Tree", field, typeof(ExpandedTree), false);
            if (field == null)
                EditorGUILayout.HelpBox("Assign a valid field", MessageType.Error);
            spField.objectReferenceValue = field;

            EditorGUILayout.PropertyField(worldPointB);

            mappedVarsFoldout = EditorGUILayout.Foldout(mappedVarsFoldout, "Mapped Vars");

            if (mappedVarsFoldout)
            {
                GUI.enabled = false;

                EditorGUILayout.PropertyField(navPointA);

                EditorGUILayout.PropertyField(navPointB);

                EditorGUILayout.PropertyField(nodeIdA);

                EditorGUILayout.PropertyField(nodeIdB);

                GUI.enabled = true;
            }

            if (EditorGUI.EndChangeCheck())
            {
                UpdateMappedPoints();
                serializedObject.ApplyModifiedProperties();
            }
        }

        void UpdateMappedPoints()
        {
            if (field == null)
                return;

            Vector2 mappedPos;
            if (field.TryMapPointToContour(jumpLinkPlacer.transform.position, out mappedPos))
            {
                navPointA.vector2Value = mappedPos;
            }
            if (field.TryMapPointToContour(worldPointB.vector2Value, out mappedPos))
            {
                navPointB.vector2Value = mappedPos;
            }

            float targetT = (navPointB.vector2Value.x - navPointA.vector2Value.x) / navAgentSettings.maxXVel;
            arcTargetJ = ((navPointB.vector2Value.y - navPointA.vector2Value.y) / targetT) + navAgentSettings.gravity * targetT;
            if (Mathf.Abs(arcTargetJ) > navAgentSettings.jumpForce)
                jumpLinkValid = false;
            else
                jumpLinkValid = true;

            //debug this arc:
            arcLowerBound = (navPointB.vector2Value.x < navPointA.vector2Value.x) ? navPointB.vector2Value.x : navPointA.vector2Value.x;
            arcUpperBound = (navPointB.vector2Value.x < navPointA.vector2Value.x) ? navPointA.vector2Value.x : navPointB.vector2Value.x;
        }

        float JumpArcCalc(float x)
        {
            x -= arcLowerBound;
            x /= navAgentSettings.maxXVel;
            return (arcTargetJ - navAgentSettings.gravity * x) * x + navPointA.vector2Value.y;
        }

        void OnSceneGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();

            worldPointB.vector2Value = Handles.PositionHandle(worldPointB.vector2Value, Quaternion.identity);
            jumpLinkPlacer.transform.position = Handles.PositionHandle(jumpLinkPlacer.transform.position, Quaternion.identity);
            if (spField.objectReferenceValue != null)
            {
                Handles.DrawDottedLine(navPointA.vector2Value, navPointB.vector2Value, 10);
                Handles.DrawLine(navPointA.vector2Value, jumpLinkPlacer.transform.position);
                Handles.DrawLine(navPointB.vector2Value, worldPointB.vector2Value);
                Handles.DrawWireDisc(navPointA.vector2Value, Vector3.forward, 0.1f);
                Handles.DrawWireDisc(navPointB.vector2Value, Vector3.forward, 0.1f);

                Handles.color = jumpLinkValid ? Color.green : Color.red;
                Vector2 swapPos;
                Vector2 prevPos = new Vector2(arcLowerBound, JumpArcCalc(arcLowerBound));
                for (float x = arcLowerBound; x + 0.1f < arcUpperBound; x += 0.1f)
                {
                    swapPos = new Vector2(x, JumpArcCalc(x));
                    Handles.DrawLine(prevPos, swapPos);
                    prevPos = swapPos;
                }
                Handles.DrawLine(prevPos, new Vector2(arcUpperBound, JumpArcCalc(arcUpperBound)));
                Handles.color = Color.white;
            }


            if (EditorGUI.EndChangeCheck() || oldPos != jumpLinkPlacer.transform.position)
            {
                UpdateMappedPoints();
                serializedObject.ApplyModifiedProperties();
            }

            oldPos = jumpLinkPlacer.transform.position;
        }*/
    }
}
