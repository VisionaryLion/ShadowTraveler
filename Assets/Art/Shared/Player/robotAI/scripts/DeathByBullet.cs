using UnityEngine;
using System.Collections;

public class DeathByBullet : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
        }
    }
}
