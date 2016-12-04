using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SmartSwitchBox))]
public class SmartSwitchBoxEditor : Editor
{

    Texture switchOnTex;
    Texture switchOffTex;
    Texture switchOnLockedTex;
    Texture switchOffLockedTex;

    SerializedProperty switchState;
    SerializedProperty switchStateLocked;
    SerializedProperty allHaveToBeOn;
    SerializedProperty keepOn;
    SerializedProperty keepOnCooldown;
    SerializedProperty keepOff;
    SerializedProperty keepOffCooldown;
    SerializedProperty connectedSwitches;
    SerializedProperty switchedOn;
    SerializedProperty switchedOff;

    bool switchedOnFoldout;
    bool switchedOffFoldout;

    void OnEnable()
    {
        switchOnTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchBoxOn.png");
        switchOffTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchBoxOff.png");
        switchOnLockedTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchBoxOn_Locked.png");
        switchOffLockedTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchBoxOff_Locked.png");

        switchState = serializedObject.FindProperty("switchState");
        switchStateLocked = serializedObject.FindProperty("isLocked");
        keepOn = serializedObject.FindProperty("_keepOn");
        keepOnCooldown = serializedObject.FindProperty("_keepOnCooldown");
        keepOff = serializedObject.FindProperty("_keepOff");
        keepOffCooldown = serializedObject.FindProperty("_keepOffCooldown");
        allHaveToBeOn = serializedObject.FindProperty("_allHaveToBeOn");
        connectedSwitches = serializedObject.FindProperty("_connectedSwitches");
        switchedOn = serializedObject.FindProperty("_switchedOn");
        switchedOff = serializedObject.FindProperty("_switchedOff");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Texture switchStateTex = switchState.boolValue ? (switchStateLocked.boolValue ? switchOnLockedTex : switchOnTex) : (switchStateLocked.boolValue ? switchOffLockedTex : switchOffTex);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(switchStateTex, GUILayout.Width(70), GUILayout.Height(70)))
        {
            switchState.boolValue = !switchState.boolValue;
        }

        GUILayout.Space(15);
        EditorGUILayout.BeginVertical();

        allHaveToBeOn.boolValue = EditorGUILayout.Toggle("All Switch On Required", allHaveToBeOn.boolValue);
        switchStateLocked.boolValue = EditorGUILayout.Toggle("Is Locked", switchStateLocked.boolValue);

        keepOn.boolValue = EditorGUILayout.Toggle("Keep On", keepOn.boolValue);
        if (keepOn.boolValue)
        {
            EditorGUI.indentLevel++;
            keepOnCooldown.floatValue = Mathf.Max(EditorGUILayout.FloatField(keepOnCooldown.floatValue == 0 ? "Cooldown (Never)" : "Cooldown", keepOnCooldown.floatValue), 0);
            EditorGUI.indentLevel--;
        }
        keepOff.boolValue = EditorGUILayout.Toggle("Keep Off", keepOff.boolValue);
        if (keepOff.boolValue)
        {
            EditorGUI.indentLevel++;
            keepOffCooldown.floatValue = Mathf.Max(EditorGUILayout.FloatField(keepOffCooldown.floatValue == 0 ? "Cooldown (Never)" : "Cooldown", keepOffCooldown.floatValue), 0);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        GUILayout.Space(15);

        EditorGUILayout.PropertyField(connectedSwitches, true);

        switchedOnFoldout = EditorGUILayout.Foldout(switchedOnFoldout, "Switched On");
        if (switchedOnFoldout)
        {
            EditorGUILayout.PropertyField(switchedOn);
        }

        switchedOffFoldout = EditorGUILayout.Foldout(switchedOffFoldout, "Switched Off");
        if (switchedOffFoldout)
        {
            EditorGUILayout.PropertyField(switchedOff);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}
