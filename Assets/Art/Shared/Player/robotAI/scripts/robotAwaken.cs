using UnityEngine;
using System.Collections;

public class robotAwaken : MonoBehaviour {

    public GameObject RealRobot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //enable real robot
            RealRobot.SetActive(true);
            //delete this robot
            this.gameObject.SetActive(false);
        }
    }
}
