using UnityEngine;
using System.Collections;

public class respawnPlayerSimple : MonoBehaviour {

    //public Transform player;
    public Transform spawnLocation;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Monster"))
        {
            //gameObject.SetActive(false);
            transform.position = spawnLocation.position;
        }
    }
}
