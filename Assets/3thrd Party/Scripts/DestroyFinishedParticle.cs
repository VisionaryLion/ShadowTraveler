using UnityEngine;
using System.Collections;

public class DestroyFinishedParticle : MonoBehaviour {

	//creates an empty particle script within the script
	private ParticleSystem thisParticleSystem;

	// Use this for initialization
	void Start () {

		//find the particle system on the object and then make it part of this particle system in the script
		thisParticleSystem = GetComponent < ParticleSystem >();
	}
	
	// Update is called once per frame
	void Update () {

		//stop here if the particle is still playing
		if (thisParticleSystem.isPlaying)
			return;

		//destroy the particle if it is done playing
		Destroy (gameObject);
	}

	//make sure the particle destroys itself after disappearing
	void OnBecameInvisible()
	{
		Destroy (gameObject);
	}
}
