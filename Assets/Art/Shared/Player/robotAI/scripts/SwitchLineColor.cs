using UnityEngine;
using System.Collections;

public class SwitchLineColor : MonoBehaviour {

    public bool HasPower = false; //add static later
    public GameObject SwitchLine;
    public Color lightOnColor;
    public Color lightOffColor;

    // Use this for initialization
    void Start () {
        if (HasPower == true)
        {
            //light is green
            SwitchLine.GetComponent<SpriteRenderer>().color = lightOnColor;
        }
        else
        {
            //light is red
            SwitchLine.GetComponent<SpriteRenderer>().color = lightOffColor;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (HasPower == true)
        {
            //light is green
            SwitchLine.GetComponent<SpriteRenderer>().color = lightOnColor;
        }
        else
        {
            //light is red
            SwitchLine.GetComponent<SpriteRenderer>().color = lightOffColor;
        }
    }
}
