using UnityEngine;
using System.Collections;

public class SpawnObject : MonoBehaviour {

	public float delay = 0.15f;
	public Transform objectParent;
	public Transform spawnPlane;
	public GameObject[] prefabs;
	
	// Used to delay spawning objects so it does not load up the memory
	float nextInput;
	
	// Use this for initialization
	void Start () {
		if (spawnPlane == null) {
			Debug.LogError("Spawn Parent must be set on the SpawnObject script !");
			enabled = false;
			return;
		}
		if (prefabs == null || prefabs.Length == 0) {
			Debug.LogError("Prefabs must be set in order to spawn objects !");
			enabled = false;
			return;
		}
		if (objectParent == null) {
			Debug.LogWarning("Object Parent should be set on the SpawnObject script : creating an empty GameObject");
			objectParent = new GameObject().transform;
			objectParent.position = Vector3.zero;
			objectParent.name = "_ObjectParent";
		}
		
		// Reset
		nextInput = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextInput && Input.GetKey(KeyCode.Space)) {
			// Use Spawn Plane position and scale to place objects randomly
			float x = Random.Range(spawnPlane.position.x - spawnPlane.localScale.x*5, spawnPlane.position.x + spawnPlane.localScale.x*5);
			float y = spawnPlane.position.y;
			float z = Random.Range(spawnPlane.position.z - spawnPlane.localScale.z*5, spawnPlane.position.z + spawnPlane.localScale.z*5);
			
			GameObject go = Instantiate(prefabs[Random.Range(0, prefabs.Length)], new Vector3(x, y, z), Quaternion.identity) as GameObject;
			go.transform.parent = objectParent;
			
			nextInput = Time.time + delay;
		}
	}
}
