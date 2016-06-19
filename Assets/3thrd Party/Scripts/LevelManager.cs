using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	//checkpoint where the player respawns
	public GameObject currentCheckpoint;

	//tells level manager who the player is
	private AnimatedPixelPack.Character player;

	//tells level manager who the enemy is
	private EnemyPatrol enemy;

	//create space in the editor for the particles 
	public GameObject deathParticle;
	public GameObject respawnParticle;

	//create float variable for player to respawn on a delay
	public float respawnDelay;

	//create space in the editor for player object
	public GameObject playerPieces;

	//create space in the editor for enemy object
	public GameObject enemyPieces;

	//variable to take points away from player after dieing
	public int pointPenaltyOnDeath;

	//variable for storing the player gravity set in the editor
	private float gravityStore;

	// Use this for initialization
	void Start () {
		player = FindObjectOfType<AnimatedPixelPack.Character> ();

		enemy = FindObjectOfType<EnemyPatrol> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//calls co-routine whenever the player dies, called from killplayer script
	public void RespawnPlayer()
	{
		StartCoroutine ("RespawnPlayerCo");
	}

	//co-routine that handles player respawn
	public IEnumerator RespawnPlayerCo()
	{
		//Creates a copy of deathparticle in the same position where the player died
		//ALWAYS put postion and rotation when instantiating an object
		Instantiate (deathParticle, player.transform.position, player.transform.rotation);

		//disable the player
		//player.enabled = false;

		//turn off visibility of player
		playerPieces.SetActive(false);

		//assign gravity scale to variable gravityStore
		gravityStore = player.GetComponent<Rigidbody2D> ().gravityScale;

		//turn off gravity on death
		player.GetComponent<Rigidbody2D>().gravityScale = 0f;

		//adds negative points to player score
		ScoreManager.AddPoints(-pointPenaltyOnDeath);

		Debug.Log ("Player Respawn");

		//wait x seconds based on the value of respawndelay in the editor
		yield return new WaitForSeconds (respawnDelay);

		//turn gravity back on after respawn
		player.GetComponent<Rigidbody2D>().gravityScale = gravityStore;

		//changes the position of the player to the position of the current checkpoint
		player.transform.position = currentCheckpoint.transform.position;

		//enable the player
		//player.enabled = true;

		//turn on visibility of player
		playerPieces.SetActive(true);

		//Creates a copy of respawnparticle in the same position where the current checkpoint is
		//ALWAYS put postion and rotation when instantiating an object
		Instantiate(respawnParticle, currentCheckpoint.transform.position, currentCheckpoint.transform.rotation);
	}

	//calls co-routine whenever the enemy dies, called from killplayer script
	public void KillEnemy()
	{
		//Creates a copy of deathparticle in the same position where the enemy died
		//ALWAYS put postion and rotation when instantiating an object
		Instantiate (deathParticle, enemy.transform.position, enemy.transform.rotation);

		//disable the enemy
		Destroy(enemy);

		//turn off visibility of enemy
		enemyPieces.SetActive(false);

		Debug.Log ("Enemy Killed");
	}
}
