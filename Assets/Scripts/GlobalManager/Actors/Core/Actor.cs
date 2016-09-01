using UnityEngine;
using System;
using System.Collections;
using System.Reflection;

namespace Actors
{
    [DisallowMultipleComponent]
    public class Actor : MonoBehaviour
    {
        void Awake()
        {
            ActorDatabase.GetInstance().AddActor(this);
#if UNITY_EDITOR
            Refresh();
#endif
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
            MonoBehaviour[] so = GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour s in so)
            {
                FieldInfo[] fInfo = s.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                FieldInfo info;
                for (int iField = 0; iField < fInfo.Length; iField++)
                {
                    info = fInfo[iField];
                    if (!info.FieldType.IsSubclassOf(typeof(Actor)))
                        continue;

                    object[] attributes = info.GetCustomAttributes(true);
                    for (int iAttr = 0; iAttr < attributes.Length; iAttr++)
                    {
                        if (attributes[iAttr].GetType() == typeof(AssignActorAutomaticly))
                        {
                            info.SetValue(s, this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Searches through every component for field marked with a "RemindToConfigureField" attribute and prints a reminder to set this field up manually.
        /// </summary>
        protected void PrintAllReminders()
        {
            MonoBehaviour[] so = GetComponentsInChildren<MonoBehaviour>();
            foreach (MonoBehaviour s in so)
            {
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
                        Debug.LogWarning(GenerateSetUpReminder(info.Name, type.Name, rtcf.addMsg));
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
    public class AssignActorAutomaticly : System.Attribute{ }
}

/// <summary>
/// Define this attribute, if you want to remind the user of a field, that has to be set up manually, when its containing script is added automatically by an Actor script.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Field)]
public class RemindToConfigureField : System.Attribute {
    /// <summary>
    /// Will print this as a new sentence in every reminder.
    /// </summary>
    public string addMsg ="";
}


