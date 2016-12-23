using UnityEngine;
using System.Collections;

public class RandomizeTransform : MonoBehaviour
{
	[SerializeField] private bool randomizePosition = false;
	[SerializeField] private bool randomizeRotation = false;
	[SerializeField] private bool randomizeScale = false;

	// Update is called once per frame
	void Update ()
	{
		if (randomizePosition)
		{
			transform.localPosition = new Vector3(Random.Range(-5f, 5f),
			                                      Random.Range(-5f, 5f),
			                                      Random.Range(-5f, 5f));
		}

		if (randomizeRotation)
		{
			transform.localRotation = Quaternion.Euler(
				new Vector3(Random.Range(0f, 359f),
			            	Random.Range(0f, 359f),
			            	Random.Range(0f, 359f)));
		}

		if (randomizeScale)
		{
			transform.localScale = new Vector3(Random.Range(-5f, 5f),
			                                   Random.Range(-5f, 5f),
			                                   Random.Range(-5f, 5f));
		}
	}
}
