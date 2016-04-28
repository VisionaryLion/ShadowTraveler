using UnityEngine;
using System.Collections;

public class MeatPickup : MonoBehaviour {

	public int pointsToAdd;

	void OnTriggerEnter2D (Collider2D other)
	{
		//if other object is NOT player then return
		if (other.GetComponent<AnimatedPixelPack.Character> () == null)
			return;

		//calls the AddPoints function in ScoreManager
		ScoreManager.AddPoints (pointsToAdd);

		//destroys the pickup
		Destroy (gameObject);
	}
}
