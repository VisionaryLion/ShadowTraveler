using UnityEngine;
using System.Collections;

public class TorchTrigger : MonoBehaviour {

	//creates empty object based on LevelManager script
	public TorchManager torchManager;

	//creates empty object based on LevelManager script
	//public TorchManager2 torchManager2;

	// Use this for initialization
	void Start () {

		//searches for any object in the scene with LevelManager script attached to it
		torchManager = FindObjectOfType<TorchManager>();

		//searches for any object in the scene with LevelManager script attached to it
		//torchManager2 = FindObjectOfType<TorchManager2>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay2D(Collider2D other)
	{
		//player is the name of the player object in the scene
		if (other.name == "Player") 
		{
			//call the respawn function in the LevelManager script
			torchManager.TurnOnLight();

			//call the respawn function in the LevelManager script
			//torchManager2.TurnOnLight();
		}
	}

	//void OnTriggerEnter2D(Collider2D other)
	//{
		//player is the name of the player object in the scene
		//if (other.name == "Player") 
		//{
			//GetComponent<Light>().intensity = 0f;
		//}
	//}
}
