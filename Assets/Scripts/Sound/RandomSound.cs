using UnityEngine;
using System.Collections;

public class RandomSound : MonoBehaviour {

    [SerializeField]
    AudioSource[] soundBank;
    [SerializeField]
    Vector2 delayParameters;

    void Start()
    {
        StartCoroutine(playSound());
    }

    float generateTime()
    {
        return Random.Range(delayParameters.x, delayParameters.y);        
    }

    IEnumerator playSound()
    {
        yield return new WaitForSeconds(generateTime());
        soundBank[Random.Range(0, soundBank.Length)].Play();
        StartCoroutine(playSound());
    }

}
