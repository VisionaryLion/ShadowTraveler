using UnityEngine;
using System.Collections;

public class PlayerSFX : MonoBehaviour {

    public static PlayerSFX playerSFX;

    public AudioClip footStepWalkSFX01;
    public AudioClip footStepWalkSFX02;
    public AudioClip footStepRunSFX01;
    public AudioClip footStepRunSFX02;
    public AudioClip footStepCrouchSFX01;
    public AudioClip footStepCrouchSFX02;
    public AudioClip JetpackSFX;
    public AudioClip GunSFX;

    public float footStepWalkVol = 0.3f * (MasterVolume.volumeHandler.gameSettings.masterVolume * MasterVolume.volumeHandler.gameSettings.sfxVolume);
    public float footStepRunVol = 0.6f * (MasterVolume.volumeHandler.gameSettings.masterVolume * MasterVolume.volumeHandler.gameSettings.sfxVolume);
    public float CrouchStepVol = 0.1f * (MasterVolume.volumeHandler.gameSettings.masterVolume * MasterVolume.volumeHandler.gameSettings.sfxVolume);
    public float JetpackSFXVol = .8f * (MasterVolume.volumeHandler.gameSettings.masterVolume * MasterVolume.volumeHandler.gameSettings.sfxVolume);
    public float GunSFXVol = 1 * (MasterVolume.volumeHandler.gameSettings.masterVolume * MasterVolume.volumeHandler.gameSettings.sfxVolume);

    AudioSource PlayerSource;
    
    void Awake()
    {
        playerSFX = this;
    }

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

    public void playGunSFX()
    {
        PlayerSource.PlayOneShot(GunSFX, GunSFXVol);
    }
}
