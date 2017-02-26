using UnityEngine;
using System.Collections;

public class Lightbulb : MonoBehaviour {

    public bool HasPower = false;
    public GameObject Light;
    public float LightOffIntensity = 0.15f;
    public float LightOnIntensity = 1.15f;

    // Use this for initialization
    void Start () {
	    if(HasPower == true)
        {
            //light is ON
            Light.GetComponent<SFLight>().intensity = LightOnIntensity;
        }
        else
        {
            //light is OFF
            Light.GetComponent<SFLight>().intensity = LightOffIntensity;
        }
    
	}
	
	// Update is called once per frame
	void Update () {
        if (HasPower == true)
        {
            //light is ON
            Light.GetComponent<SFLight>().intensity = LightOnIntensity;
        }
        else
        {
            //light is OFF
            Light.GetComponent<SFLight>().intensity = LightOffIntensity;
        }
    }

    public void ChangeLightIntensity()
    {
        if (HasPower == true)
        {
            //light is ON
            Light.GetComponent<SFLight>().intensity = LightOffIntensity;
            //HasPower = false;
        }
        else
        {
            //light is OFF
            Light.GetComponent<SFLight>().intensity = LightOnIntensity;
            //HasPower = true;
        }
    }
}
