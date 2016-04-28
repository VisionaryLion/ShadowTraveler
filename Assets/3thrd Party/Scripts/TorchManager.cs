using UnityEngine;
using System.Collections;

public class TorchManager : MonoBehaviour {

	//light intensity
	public float fadeSpeed = 2f;           // How fast the light fades between intensities
	public float highIntensity = 6f;       // The maximum intensity of the light while on
	public float lowIntensity = 0.5f;      // The minumum intensity of the light while on
	public float changeMargin = 0.2f;      // The margin in which the target intensity changes
	public bool torchOn;                   // Whether or not the torch is on

	private float targetIntensity;         // The intensity that the light is aiming for currently

	public Light rightLight;
	public Light leftLight;

	//light collider size
	public float scaleSpeed = 2f;           // How fast the light collider changes size
	public float bigSize = 2f;       // The maximum size of the light collider while on
	public float smallSize = 0.5f;      // The minumum size of the light collider while on
	public float sizeMargin = 0.2f;      // The margin in which the target size changes

	private float targetSize;         // The size that the light collider is aiming for currently

	public CircleCollider2D circleCollider;

	//fire particle size
	public float scaleSpeedP = 0.01f;           // How fast the light fades between intensities
	public float bigPSize = 0.08f;       // The maximum intensity of the light while on
	public float smallPSize = 0.0f;      // The minumum intensity of the light while on
	public float psizeMargin = 0.2f;      // The margin in which the target intensity changes

	private float targetPSize;         // The intensity that the light is aiming for currently

	public ParticleSystem torchFire;

	void Awake ()
	{
		// When the level starts we want the light to be "off"
		rightLight.intensity = 0f;
		leftLight.intensity = 0f;

		//When the alarm starts for the first time, the light should aim to have the maximum intensity
		targetIntensity = highIntensity;

		// When the level starts we want the light collider to be "off"
		circleCollider.radius = 0f;

		//When the alarm starts for the first time, the light collider should aim to have the biggest size
		targetSize = bigSize;

		// When the level starts we want the fire particle to be "off"
		torchFire.startSize = 0f;

		//When the alarm starts for the first time, the fire particle should aim to have the biggest size
		targetPSize = bigPSize;
	}

	void Update ()
	{
		// If the light is on...
		if (torchOn) 
		{
			// ... Lerp the lights intensity towards the current target
			rightLight.intensity = Mathf.Lerp(rightLight.intensity, targetIntensity, fadeSpeed * Time.deltaTime);
			leftLight.intensity = Mathf.Lerp(leftLight.intensity, targetIntensity, fadeSpeed * Time.deltaTime);

			// check whether the target intensity needs changing and change it if so
			CheckTargetIntensity();

			// ... Lerp the light colliders size towards the current target
			circleCollider.radius = Mathf.Lerp(circleCollider.radius, targetSize, scaleSpeed * Time.deltaTime);

			// check whether the target size of the collider needs changing and change it if so
			CheckTargetSize();

			// ... Lerp the fire particle size towards the current target
			torchFire.startSize = Mathf.Lerp(torchFire.startSize, targetPSize, scaleSpeedP * Time.deltaTime);

			// check whether the target size of the particle needs changing and change it if so
			CheckTargetPSize();
		}
		else
			// otherwise fade the lights intensity to zero
			rightLight.intensity = Mathf.Lerp(rightLight.intensity, 0f, fadeSpeed * Time.deltaTime);
		    leftLight.intensity = Mathf.Lerp(leftLight.intensity, 0f, fadeSpeed * Time.deltaTime);

		   // otherwise fade the light colliders size to zero
		   circleCollider.radius = Mathf.Lerp(circleCollider.radius, 0f, scaleSpeed * Time.deltaTime);

		   // otherwise fade the particle to zero
		   torchFire.startSize = Mathf.Lerp(torchFire.startSize, 0f, scaleSpeedP * Time.deltaTime);
	}

	//check the lights intensity
	void CheckTargetIntensity()
	{
		// if the difference between the target and current intensities is less than the change margin...
		if (Mathf.Abs (targetIntensity - rightLight.intensity) < changeMargin) 
		{
			// ... if the target intensity is high ...
			if(targetIntensity == highIntensity)
				// ... then set the target to low
				targetIntensity = lowIntensity;
			else
				// otherwise set the target to high
				targetIntensity = highIntensity;
		}

		// if the difference between the target and current intensities is less than the change margin...
		if (Mathf.Abs (targetIntensity - leftLight.intensity) < changeMargin) 
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

	//check the size of the light circle
	void CheckTargetSize()
	{
		// if the difference between the target and current size is less than the size margin...
		if (Mathf.Abs (targetSize - circleCollider.radius) < sizeMargin) 
		{
			// ... if the target size is big ...
			if(targetSize == bigSize)
				// ... then set the target to small
				targetSize = smallSize;
			else
				// otherwise set the target to small
				targetSize = bigSize;
		}
	}

	//check the size of the fire particle
	void CheckTargetPSize()
	{
		// if the difference between the target and current size is less than the size margin...
		if (Mathf.Abs (targetPSize - torchFire.startSize) < psizeMargin) 
		{
			// ... if the target size is big ...
			if(targetPSize == bigPSize)
				// ... then set the target to small
				targetPSize = smallPSize;
			else
				// otherwise set the target to small
				targetPSize = bigPSize;
		}
	}

	//turns the lights, collider, and particle on after running into TorchTrigger
	public void TurnOnLight()
	{
		rightLight.intensity = highIntensity;
		leftLight.intensity = highIntensity;
		circleCollider.radius = bigSize;
		torchFire.startSize = bigPSize;
	}

}
