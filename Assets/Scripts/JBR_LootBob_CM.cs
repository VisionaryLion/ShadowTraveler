using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]


public class JBR_LootBob_CM : MonoBehaviour {
	[Tooltip("LifeTime of gameObject , if set to zero no lifetime is used")]
	public float goLifeTime = 10.0f; //GameObject lifetime
	private float timer;
	private SphereCollider sphereCol;

	// position / rotation
	protected Vector3 m_SpawnPosition = Vector3.zero;
	protected Transform m_Transform = null;
	[Tooltip("Bob up and down, Distance, set to zero if no bob is wanted")]
	public float BobAmp = 0.5f;						// air floating strength
	[Tooltip("Bob up and down, Speed, set to zero if no bob is wanted")]
	public float BobRate = 0.5f;					// air floating speed
	public float BobOffset = -1.0f;					// bob offset for making pickups in a row float independently
	public float rotationSpeed = 90.0f;

	void Start(){
		//make sure this gameobject has a collider
		sphereCol = this.gameObject.GetComponent<SphereCollider> ();
		if (sphereCol == null) {
			sphereCol = this.gameObject.AddComponent<SphereCollider> ();
		}
		sphereCol.isTrigger = true;
		sphereCol.radius = .5f;

		BobOffset = UnityEngine.Random.Range (-1.0f, 1.0f);
	}

	// Use this for initialization 
	void OnEnable () {
		// start despawn timer
		if (goLifeTime != 0) {
			timer = 0.01f;
		}
		//
		m_Transform = transform;
		m_SpawnPosition = m_Transform.position;
	}

	// Update is called once per frame
	void Update () {
		// if we have bob values, make the pickup float up and down in the air
		if (BobRate != 0.0f && BobAmp != 0.0f)
			m_Transform.position = m_SpawnPosition + Vector3.up *
				(Mathf.Cos((Time.time + BobOffset) * (BobRate * 10.0f)) * BobAmp);

		if(rotationSpeed != 0){
			m_Transform.Rotate(0,(Time.deltaTime ) * rotationSpeed,0);
		}
		// despawn timer
		if (timer > 0) {
			timer += Time.deltaTime;
		}
		if (timer >= goLifeTime) {
			timer = 0;
		//	Destroy (this.gameObject);
		//	this.gameObject.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.tag == ("Player")) {
			Debug.LogAssertion ("Player touched this");

			timer = 0;
			this.gameObject.SetActive(false);
			//	Destroy (this.gameObject);
		}
	}
}