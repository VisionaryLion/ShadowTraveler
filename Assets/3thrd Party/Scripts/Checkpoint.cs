using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	//creates empty object based on LevelManager script
	public LevelManager levelManager;

	// Use this for initialization
	void Start () {

		//searches for any object in the scene with LevelManager script attached to it
		levelManager = FindObjectOfType<LevelManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//player is the name of the player object in the scene
		if (other.name == "Player") 
		{
			//assigns currentCheckpoint to whatever gameobject the Checkpoint script is attached to
			levelManager.currentCheckpoint = gameObject;
			Debug.Log ("Activated Checkpoint " + transform.position);
		}
	}
}
