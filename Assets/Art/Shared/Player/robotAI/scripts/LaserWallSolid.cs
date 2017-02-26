using UnityEngine;
using System.Collections;

public class LaserWallSolid : MonoBehaviour {

    public bool HasPower = true;
    public GameObject LaserBeam;

    // Use this for initialization
    void Start () {
        if (HasPower == true)
        {
            //wall is DOWN
            LaserBeam.SetActive(true);
        }
        else
        {
            //wall is UP
            LaserBeam.SetActive(false);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (HasPower == true)
        {
            //wall is DOWN
            LaserBeam.SetActive(true);
        }
        else
        {
            //wall is UP
            LaserBeam.SetActive(false);
        }
    }
}
