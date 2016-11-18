using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(InteractiveInputUIMarker))]
public class InteractiveInputUIMarkerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Refresh Auto"))
        {
            InteractiveInputUIMarker marker = (InteractiveInputUIMarker)target;
            Text[] texts = marker.gameObject.GetComponentsInChildren<Text>(true);
            InteractiveInputUIMarker.UIInputItem[] items = new InteractiveInputUIMarker.UIInputItem[texts.Length];

            for (int iText = 0; iText < texts.Length; iText++)
            {
                Text t = texts[iText];
                Image image = t.transform.parent.GetComponentInChildren<Image>(true);
                items[iText] = new InteractiveInputUIMarker.UIInputItem();
                items[iText].icon = image;
                items[iText].description = t;
                items[iText].itemRoot = t.transform.parent.gameObject;
            }

            marker.uiItemQueue = items;
        }
    }
}
