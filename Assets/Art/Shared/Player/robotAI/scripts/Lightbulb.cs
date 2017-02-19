using UnityEngine;
using System.Collections;

public class Lightbulb : MonoBehaviour {

    public bool LightHasPower = false;
    public GameObject Light;
    public float LightOffIntensity = 0.15f;
    public float LightOnIntensity = 1.15f;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (LightHasPower == true)
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
}
