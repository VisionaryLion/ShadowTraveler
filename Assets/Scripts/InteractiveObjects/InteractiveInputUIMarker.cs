using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InteractiveInputUIMarker : MonoBehaviour {

    public UIInputItem[] uiItemQueue;

    [System.Serializable]
    public class UIInputItem
    {
        public Image icon;
        public Text description;
        public GameObject itemRoot;

        public void SetVisible(bool visible)
        {
            itemRoot.SetActive(visible);
        }

        public void UpdateContent(InteractiveInputDefinition def)
        {
                icon.sprite = def.icon;
                description.text = def.description;
        }
    }
}
