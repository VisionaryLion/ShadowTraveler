using UnityEngine;
using System.Collections;

public class TargetManager : MonoBehaviour {

    //public PlayerHealth playerHealth;       // Reference to the player's heatlh.
    [Tooltip("The enemy prefab to be spawned")]
    public GameObject target;                // The enemy prefab to be spawned.
    [Tooltip("The total number of enemies spawned across the map")]
    public int TotalAllowedEnemies = 15;
    public static int TotalEnemies = 0;
    [Tooltip("The duration between each spawn")]
    public float spawnTime = 2f;            // How long between each spawn.
    [Tooltip("The array of spawn points. ")]
    public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
    
    

    void Start()
    {        
        // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
        InvokeRepeating("Spawn", 0, spawnTime);
    }


    void Spawn()
    {
        // If the player has no health left...
        //if (playerHealth.currentHealth <= 0f)
        //{
        // ... exit the function.
        //    return;
        //}
        if (TotalEnemies <= TotalAllowedEnemies)
        {
            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range(0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
            Instantiate(target, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
            TotalEnemies = TotalEnemies + 1;
        }
    }
}