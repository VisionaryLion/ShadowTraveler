//Add to same object that holds the Inventory.cs component
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private ItemInterface item;
    private string data;
    private GameObject tooltip;

    void Start()
    {
        tooltip = GameObject.Find("Tooltip"); //TODO: Find a better way
        tooltip.SetActive(false);
    }

    void Update()
    {
        if(tooltip.activeSelf)
        {
            tooltip.transform.position = Input.mousePosition;
        }
    }

    public void Activate(ItemInterface item)
    {
        this.item = item;
        Buildstring();
        tooltip.SetActive(true);
    }

    public void Deactivate()
    {
        tooltip.SetActive(false);
    }

    public void Buildstring()
    {
        data = item.title;
        tooltip.GetComponentInChildren<Text>().text = data;
    }
}
