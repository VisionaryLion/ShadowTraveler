using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(SmartTriggerSwitch))]
public class SmartTriggerSwitchEditor : Editor
{

    Texture switchOnTex;
    Texture switchOffTex;
    Texture switchOnLockedTex;
    Texture switchOffLockedTex;

    SerializedProperty switchState;
    SerializedProperty switchStateLocked;
    SerializedProperty keepOn;
    SerializedProperty keepOnCooldown;
    SerializedProperty keepOff;
    SerializedProperty keepOffCooldown;
    SerializedProperty activatorGroups;
    SerializedProperty switchedOn;
    SerializedProperty switchedOff;

    ReorderableList activatorList;

    bool activatorGroupsFoldout;
    bool switchedOnFoldout;
    bool switchedOffFoldout;

    void OnEnable()
    {
        activatorGroupsFoldout = EditorPrefs.GetBool("SmartSwitch_Global_Foldout_Activator", false);
        switchedOnFoldout = EditorPrefs.GetBool("SmartSwitch_Global_Foldout_SwitchedOn", false);
        switchedOffFoldout = EditorPrefs.GetBool("SmartSwitch_Global_Foldout_SwitchedOff", false);

        switchOnTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchOn.png");
        switchOffTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchOff.png");
        switchOnLockedTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchOn_Locked.png");
        switchOffLockedTex = (Texture)EditorGUIUtility.Load("SmartSwitch/SmartSwitchOff_Locked.png");

        switchState = serializedObject.FindProperty("_switchState");
        switchStateLocked = serializedObject.FindProperty("_switchLocked");
        keepOn = serializedObject.FindProperty("_keepOn");
        keepOnCooldown = serializedObject.FindProperty("_keepOnCooldown");
        keepOff = serializedObject.FindProperty("_keepOff");
        keepOffCooldown = serializedObject.FindProperty("_keepOffCooldown");
        activatorGroups = serializedObject.FindProperty("_activatorGroups");
        switchedOn = serializedObject.FindProperty("_switchedOn");
        switchedOff = serializedObject.FindProperty("_switchedOff");

        activatorList = new ReorderableList(serializedObject, null, false, true, true, true);
        activatorList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = activatorList.serializedProperty.GetArrayElementAtIndex(index);
            SerializedProperty activeActivator = element.FindPropertyRelative("_activeActivator");
            SerializedProperty inverseCondition = element.FindPropertyRelative("_inverseCondition");

            rect.y += EditorGUIUtility.standardVerticalSpacing;
            float widthScale = (rect.width - 125) / 125;
            float width;

            EditorGUI.BeginChangeCheck();

            activeActivator.intValue = EditorGUI.Popup(new Rect(rect.x, rect.y, 120, EditorGUIUtility.singleLineHeight), activeActivator.intValue, new string[] { "ObjectRecognicion", "Layer", "Tag", "Actor", "Has Item", "Key" });


            EditorGUIUtility.labelWidth = Mathf.Min(50 * widthScale, 50);
            width = Mathf.Min(50 * widthScale, 50) + 10;

            inverseCondition.boolValue = EditorGUI.Toggle(new Rect(rect.width - EditorGUIUtility.labelWidth, rect.y, width, EditorGUIUtility.singleLineHeight), "Inverse", inverseCondition.boolValue);

            GUI.enabled = !inverseCondition.boolValue;
            EditorGUIUtility.labelWidth = Mathf.Min(60 * widthScale, 60);
            rect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            if (activeActivator.intValue == 0)
            {
                SerializedProperty activateObj = element.FindPropertyRelative("_activateGameObjectRec");
                SerializedProperty objectToRecognize = activateObj.FindPropertyRelative("_objectToRecognize");
                SerializedProperty onLeft = activateObj.FindPropertyRelative("_onLeave");

                if (inverseCondition.boolValue)
                    onLeft.boolValue = false;

                EditorGUI.PropertyField(new Rect(rect.width - width - 5 - EditorGUIUtility.labelWidth, rect.y - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), Mathf.Min(60 * widthScale, 60) + 10, EditorGUIUtility.singleLineHeight), onLeft);

                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight), objectToRecognize);
            }
            else if (activeActivator.intValue == 1)
            {
                SerializedProperty activateLayer = element.FindPropertyRelative("_activateLayer");
                SerializedProperty layerToRec = activateLayer.FindPropertyRelative("_layer");
                SerializedProperty onLeft = activateLayer.FindPropertyRelative("_onLeave");

                if (inverseCondition.boolValue)
                    onLeft.boolValue = false;

                EditorGUI.PropertyField(new Rect(rect.width - width - 5 - EditorGUIUtility.labelWidth, rect.y - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), Mathf.Min(60 * widthScale, 60) + 10, EditorGUIUtility.singleLineHeight), onLeft);

                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight), layerToRec);
            }
            else if (activeActivator.intValue == 2)
            {
                SerializedProperty activateTag = element.FindPropertyRelative("_activateTag");
                SerializedProperty tagToRec = activateTag.FindPropertyRelative("_tag");
                SerializedProperty onLeft = activateTag.FindPropertyRelative("_onLeave");

                if (inverseCondition.boolValue)
                    onLeft.boolValue = false;

                EditorGUI.PropertyField(new Rect(rect.width - width - 5 - EditorGUIUtility.labelWidth, rect.y - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), Mathf.Min(60 * widthScale, 60) + 10, EditorGUIUtility.singleLineHeight), onLeft);

                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight), tagToRec);
            }
            else if (activeActivator.intValue == 3)
            {
                SerializedProperty activateActor = element.FindPropertyRelative("_activateActor");
                SerializedProperty actorToRec = activateActor.FindPropertyRelative("staticActorTypeIndex");
                SerializedProperty onLeft = activateActor.FindPropertyRelative("_onLeave");

                if (inverseCondition.boolValue)
                    onLeft.boolValue = false;

                EditorGUI.PropertyField(new Rect(rect.width - width - 5 - EditorGUIUtility.labelWidth, rect.y - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), Mathf.Min(60 * widthScale, 60) + 10, EditorGUIUtility.singleLineHeight), onLeft);

                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                actorToRec.intValue = EditorGUI.Popup(new Rect(rect.x, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight), actorToRec.intValue, Entities.Entity.EntitySubtypeNames);
            }
            else if (activeActivator.intValue == 4)
            {
                SerializedProperty activateItem = element.FindPropertyRelative("_activateItem");
                SerializedProperty itemIdToRec = activateItem.FindPropertyRelative("item");
                SerializedProperty onLeft = activateItem.FindPropertyRelative("_onLeave");

                if (inverseCondition.boolValue)
                    onLeft.boolValue = false;

                EditorGUI.PropertyField(new Rect(rect.width - width - 5 - EditorGUIUtility.labelWidth, rect.y - (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing), Mathf.Min(60 * widthScale, 60) + 10, EditorGUIUtility.singleLineHeight), onLeft);

                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, EditorGUIUtility.singleLineHeight), itemIdToRec, new GUIContent("Item"));
            }
            else if (activeActivator.intValue == 5)
            {
                SerializedProperty activateKey = element.FindPropertyRelative("_activateKey");
                SerializedProperty keyToRec = activateKey.FindPropertyRelative("interactiveInputDef");
                SerializedProperty keyPressType = activateKey.FindPropertyRelative("pressType");
                SerializedProperty switchCallback = activateKey.FindPropertyRelative("switchCallback");
                GUI.enabled = true;
                EditorGUIUtility.labelWidth = 130;

                if (switchCallback.objectReferenceValue != target)
                    switchCallback.objectReferenceValue = target;
                width = (rect.width - 10) / 3f;
                EditorGUIUtility.labelWidth = 70;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, width, EditorGUIUtility.singleLineHeight), keyPressType);
                EditorGUI.PropertyField(new Rect(rect.x + width + 5, rect.y, width * 2, EditorGUIUtility.singleLineHeight), keyToRec);
            }


            if (EditorGUI.EndChangeCheck())
            {
                activatorList.serializedProperty.serializedObject.ApplyModifiedProperties();
            }
        };

        activatorList.onAddCallback = (ReorderableList list) =>
        {
            var index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.FindPropertyRelative("_activeActivator").intValue = 0;
            element.FindPropertyRelative("_inverseCondition").boolValue = false;
            list.serializedProperty.serializedObject.ApplyModifiedProperties();
        };
        activatorList.elementHeight = EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing * 3;
        activatorList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Activator Group");
        };
    }

    void OnDestroy()
    {
        EditorPrefs.SetBool("SmartSwitch_Global_Foldout_Activator", activatorGroupsFoldout);
        EditorPrefs.SetBool("SmartSwitch_Global_Foldout_SwitchedOn", switchedOnFoldout);
        EditorPrefs.SetBool("SmartSwitch_Global_Foldout_SwitchedOff", switchedOffFoldout);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Texture switchStateTex = switchState.boolValue ? (switchStateLocked.boolValue ? switchOnLockedTex : switchOnTex) : (switchStateLocked.boolValue ? switchOffLockedTex : switchOffTex);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(switchStateTex, GUILayout.Width(70), GUILayout.Height(70)))
        {
            if (EditorApplication.isPlaying)
            {
                if (switchState.boolValue)
                    ((SmartTriggerSwitch)target).FlipSwitchOff();
                else
                    ((SmartTriggerSwitch)target).FlipSwitchOn();
            }
            else
                switchState.boolValue = !switchState.boolValue;
        }

        GUILayout.Space(15);
        EditorGUILayout.BeginVertical();

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

        GUI.enabled = true;

        if (activatorGroups.arraySize == 0)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Activator Groups");
            if (GUILayout.Button("+", GUILayout.Width(50)))
            {
                activatorGroups.arraySize++;
                activatorGroups.GetArrayElementAtIndex(activatorGroups.arraySize - 1).FindPropertyRelative("_activators").arraySize = 0;
            }
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.BeginHorizontal();
            activatorGroupsFoldout = EditorGUILayout.Foldout(activatorGroupsFoldout, "Activator Groups:");
            if (activatorGroupsFoldout)
            {
                if (GUILayout.Button("+", GUILayout.Width(50)))
                {
                    activatorGroups.arraySize++;
                    activatorGroups.GetArrayElementAtIndex(activatorGroups.arraySize - 1).FindPropertyRelative("_activators").arraySize = 0;
                }
                EditorGUILayout.EndHorizontal();

                for (int iGroup = 0; iGroup < activatorGroups.arraySize; iGroup++)
                {
                    SerializedProperty element = activatorGroups.GetArrayElementAtIndex(iGroup).FindPropertyRelative("_activators");
                    activatorList.serializedProperty = element;
                    activatorList.DoLayoutList();

                    Rect lastRect = GUILayoutUtility.GetLastRect();
                    if (GUI.Button(new Rect(lastRect.width - 5, lastRect.y - (activatorList.GetHeight() - 15), 15, 15), "x"))
                    {
                        activatorGroups.MoveArrayElement(iGroup, activatorGroups.arraySize - 1);
                        activatorGroups.arraySize--;
                        iGroup--;
                        continue;
                    }
                }
            }
            else
                EditorGUILayout.EndHorizontal();
        }

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
