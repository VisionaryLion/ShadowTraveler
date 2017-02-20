using UnityEngine;
using System.Collections;

public class PlayerSFX : MonoBehaviour {

    public AudioClip footStepWalkSFX01;
    public AudioClip footStepWalkSFX02;
    public AudioClip footStepRunSFX01;
    public AudioClip footStepRunSFX02;
    public AudioClip footStepCrouchSFX01;
    public AudioClip footStepCrouchSFX02;
    public AudioClip JetpackSFX;

    public float footStepWalkVol = 0.3f;
    public float footStepRunVol = 0.6f;
    public float CrouchStepVol = 0.1f;
    public float JetpackSFXVol = .8f;

    AudioSource PlayerSource;

    void Start () {
        PlayerSource = GetComponent<AudioSource>();
    }

    void playfootStepWalkSFX01()
    {
        PlayerSource.PlayOneShot(footStepWalkSFX01, footStepWalkVol);
    }

    void playfootStepWalkSFX02()
    {
        PlayerSource.PlayOneShot(footStepWalkSFX02, footStepWalkVol);
    }

    void playfootStepRunSFX01()
    {
        PlayerSource.PlayOneShot(footStepRunSFX01, footStepRunVol);
    }

    void playfootStepRunSFX02()
    {
        PlayerSource.PlayOneShot(footStepRunSFX02, footStepRunVol);
    }

    void playfootStepCrouchSFX01()
    {
        PlayerSource.PlayOneShot(footStepCrouchSFX01, CrouchStepVol);
    }

    void playfootStepCrouchSFX02()
    {
        PlayerSource.PlayOneShot(footStepCrouchSFX02, CrouchStepVol);
    }

    void playJetpackSFX()
    {
        PlayerSource.PlayOneShot(JetpackSFX, JetpackSFXVol);
    }
}
