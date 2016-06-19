using UnityEngine;
using System.Collections;

public class KillPlayer : MonoBehaviour {

	//creates empty object based on LevelManager script
	public LevelManager levelManager;

	//tells script who the enemy is
	private EnemyPatrol enemy;

	// Use this for initialization
	void Start () {

		//searches for any object in the scene with LevelManager script attached to it
		levelManager = FindObjectOfType<LevelManager>();

		enemy = FindObjectOfType<EnemyPatrol> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//player is the name of the player object in the scene
		if (other.name == "Player") 
		{
			//call the respawn function in the LevelManager script
			levelManager.RespawnPlayer();
		}
			
		if (other.name == "ZombiePatrol") 
		{
			levelManager.KillEnemy();
		}
	}
}
