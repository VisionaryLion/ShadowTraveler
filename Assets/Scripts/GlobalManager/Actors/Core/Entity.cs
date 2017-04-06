﻿using UnityEngine;
using System;
using System.Reflection;
using UnityEditor;

namespace Entities
{
    [SelectionBase]
    public class Entity : MonoBehaviour
    {
        public static readonly Type[] EntitySubtypes = new Type[] {
            typeof(Entity),
            typeof(ActingEntity),
            typeof(ElevatorEntity),
            typeof(GameStateEntity),
            typeof(HealthEntity),
            typeof(ItemEntity),
            typeof(MovingPlatformEntity),
            typeof(SimpleMovingEntity),
            typeof(AnimationEntity),
            typeof(HumanMovementEntity),
            typeof(ActingEquipmentEntity),
            typeof(PlayerEntity)
            };

        public static readonly string[] EntitySubtypeNames = new string[] {
            "Entity",
            "ActingEntity",
            "ElevatorEntity",
            "GameStateEntity",
            "HealthEntity",
            "ItemEntity",
            "MovingPlatformEntity",
            "SimpleMovingEntity",
            "AnimationEntity",
            "HumanMovementEntity",
            "ActingEquipmentEntity",
            "PlayerEntity"
            };

        public static int EntityTypeToStaticIndex (Type t)
        {
            for (int iType = 0; iType < EntitySubtypes.Length; iType++)
            {
                if (EntitySubtypes[iType].Equals(t))
                    return iType;
            }
            Debug.LogError("Unkown Entity type = "+t);
            return -1;
        }

        protected virtual void Awake()
        {
            EntityDatabase.GetInstance().AddEntity(this);
#if UNITY_EDITOR
            PrintAllReminders();
            FillRefAutomaticly();
#endif
        }

        protected virtual void OnDestroy()
        {
            EntityDatabase.GetInstance().RemoveEntity(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Base call strongly suggested!
        /// </summary>
        public virtual void Refresh()
        {
            MoveThisComponentToTop();
            PrintAllReminders();
            FillRefAutomaticly();
        }

        protected void FillRefAutomaticly()
        {
            MonoBehaviour[] so = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour s in so)
            {
                if (s == null || s.GetType().IsSubclassOf(typeof(Entity)))
                    continue;
                FieldInfo[] fInfo = s.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                FieldInfo info;
                for (int iField = 0; iField < fInfo.Length; iField++)
                {
                    info = fInfo[iField];

                    if (!GetType().IsSubclassOf(info.FieldType) && info.FieldType != GetType())
                    {
                        continue;
                    }

                    object[] attributes = info.GetCustomAttributes(true);
                    for (int iAttr = 0; iAttr < attributes.Length; iAttr++)
                    {
                        if (attributes[iAttr].GetType() == typeof(AssignEntityAutomaticly))
                        {
                            info.SetValue(s, this);
                        }
                    }
                }
            }
        }

        protected T LoadComponent<T>(string fieldName, T oldComp) where T : Component
        {
            T[] components = GetComponentsInChildren<T>();
            if (components == null)
                return null;
            if (components.Length == 1)
                return components[0];

            if (oldComp != null)
            {
                if (EditorUtility.DisplayDialog("Multiple Components Found", "Found (" + components.Length + ") components of type " + typeof(T) + " for " + fieldName + ".\nKeep the old one (" + oldComp.gameObject.name + ")?", "Yes", "No"))
                {
                    return oldComp;
                }
            }

            for (int iCom = 0; iCom < components.Length; iCom++)
            {
                if (EditorUtility.DisplayDialog("Multiple Components Found", "Found (" + components.Length + ") components of type " + typeof(T) + " for " + fieldName + ".\nFill it with " + components[iCom].gameObject.name + "?", "Yes", (iCom == components.Length - 1) ? "Assign Null" : "Next"))
                {
                    return components[iCom];
                }
            }
            return null;
        }

        protected T LoadComponent<T>(T oldComp) where T : Component
        {
            return LoadComponent<T>(typeof(T).ToString(), oldComp);
        }

        /// <summary>
        /// Searches through every component for field marked with a "RemindToConfigureField" attribute and prints a reminder to set this field up manually.
        /// </summary>
        protected void PrintAllReminders()
        {
            MonoBehaviour[] so = GetComponentsInChildren<MonoBehaviour>(true);
            foreach (MonoBehaviour s in so)
            {
                if (s != null)
                    PrintReminder(s.GetType());
            }
        }

        /// <summary>
        /// Uses reflection to find all fields with a "RemindToConfigureField" attribute and prints a helpful warning to the User, using Debug.Warning.
        /// </summary>
        /// <param name="obj">The type to search through.</param>
        protected static void PrintReminder(Type type)
        {
            FieldInfo[] fInfo = type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            MemberInfo info;
            for (int iField = 0; iField < fInfo.Length; iField++)
            {
                info = fInfo[iField];
                object[] attributes = info.GetCustomAttributes(true);
                for (int iAttr = 0; iAttr < attributes.Length; iAttr++)
                {
                    if (attributes[iAttr].GetType() == typeof(RemindToConfigureField))
                    {
                        RemindToConfigureField rtcf = (RemindToConfigureField)attributes[iAttr];
                        //Debug.LogWarning(GenerateSetUpReminder(info.Name, type.Name, rtcf.addMsg));
                    }
                }
            }
        }

        /// <summary>
        /// Generates a friendly reminder, to set up the "field", which is contained by the "type".
        /// </summary>
        /// <param name="field">The "field", the User should set up manually.</param>
        /// <param name="type">The "type", which contains the field, so the User know where to look.</param>
        /// <returns>The reminder.</returns>
        protected static string GenerateSetUpReminder(string field, string type, string additionalMsg = "")
        {
            return "Don't forget to define  <color=blue>\"" + field + "\"</color> within the <color=blue>\"" + type + "\"</color> script. " + additionalMsg;
        }

        /// <summary>
        /// Generates a friendly reminder, to set up the "field" A mention of the containing type is omitted.
        /// </summary>
        /// <param name="field">The "field", the User should set up manually.</param>
        /// <returns>The reminder.</returns>
        protected static string GenerateSetUpReminderShort(string field, string additionalMsg = "")
        {
            return "Don't forget to define <color=blue>\"" + field + "\"</color>. " + additionalMsg;
        }

        /// <summary>
        /// Will move this component to the top, so it appears in the inspector first.
        /// </summary>
        protected void MoveThisComponentToTop()
        {
            int componentLength = GetComponents<MonoBehaviour>().Length;
            for (int i = 0; i < componentLength; i++)
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this); //Move this script up to the top in the hierarchy!
        }
#endif
    }


    /// <summary>
    /// Will fill a var tagged with this attribute with a reference to the local actor.
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class AssignEntityAutomaticly : System.Attribute { }
}

/// <summary>
/// Define this attribute, if you want to remind the user of a field, that has to be set up manually, when its containing script is added automatically by an Actor script.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class RemindToConfigureField : System.Attribute
{
    /// <summary>
    /// Will print this as a new sentence in every reminder.
    /// </summary>
    public string addMsg = "";
}


