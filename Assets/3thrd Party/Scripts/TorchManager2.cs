using UnityEngine;
using System.Collections;

public class TorchManager2 : MonoBehaviour {

	public float fadeSpeed = 2f;           // How fast the light fades between intensities
	public float highIntensity = 6f;       // The maximum intensity of the light while on
	public float lowIntensity = 0.5f;      // The minumum intensity of the light while on
	public float changeMargin = 0.2f;      // The margin in which the target intensity changes
	public bool torchOn;                   // Whether or not the torch is on

	private float targetIntensity;         // The intensity that the light is aiming for currently

	void Awake ()
	{
		// When the level starts we want the light to be "off"
		GetComponent<Light>().intensity = 0f;

		//When the alarm starts for the first time, the light should aim to have the maximum intensity
		targetIntensity = highIntensity;
	}

	void Update ()
	{
		// If the light is on...
		if (torchOn) 
		{
			// ... Lerp the lights intensity towards the current target
			GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, targetIntensity, fadeSpeed * Time.deltaTime);

			// check whether the target intensity needs changing and change it if so
			CheckTargetIntensity();
		}
		else
			// otherwise fade the lights intensity to zero
			GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, 0f, fadeSpeed * Time.deltaTime);
	}

	void CheckTargetIntensity()
	{
		// if the difference between the target and current intensities is less than the change margin...
		if (Mathf.Abs (targetIntensity - GetComponent<Light> ().intensity) < changeMargin) 
		{
			// ... if the target intensity is high ...
			if(targetIntensity == highIntensity)
				// ... then set the target to low
				targetIntensity = lowIntensity;
			else
				// otherwise set the target to high
				targetIntensity = highIntensity;
		}
	}

	public void TurnOnLight()
	{
		GetComponent<Light>().intensity = 6f;
	}

}
