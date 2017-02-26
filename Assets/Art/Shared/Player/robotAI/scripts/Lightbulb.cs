using UnityEngine;
using System.Collections;

public class Lightbulb : MonoBehaviour {

    [SerializeField]
    private bool LightHasPower = false;
    public GameObject Light;
    public Renderer lightBulbMat;
    public float LightOffIntensity = 0;
    public float LightOnIntensity = 1.15f;

    // Use this for initialization
    void Start () {
        if (LightHasPower == true)
        {
            //light is ON
            Light.GetComponent<SFLight>().intensity = LightOnIntensity;
            lightBulbMat.material.SetFloat("_LightAffectionIntensity", 0);
        }
        else
        {
            //light is OFF
            Light.GetComponent<SFLight>().intensity = LightOffIntensity;
            lightBulbMat.material.SetFloat("_LightAffectionIntensity", 1);
        }
    }

    public void ToogleStatus(bool state)
    {
        if (state == LightHasPower)
            return;
        LightHasPower = state;
        if (LightHasPower == true)
        {
            //light is ON
            Light.GetComponent<SFLight>().intensity = LightOnIntensity;
            lightBulbMat.material.SetFloat("_LightAffectionIntensity", 0);
        }
        else
        {
            //light is OFF
            Light.GetComponent<SFLight>().intensity = LightOffIntensity;
            lightBulbMat.material.SetFloat("_LightAffectionIntensity", 1);
        }
        
    }
}
