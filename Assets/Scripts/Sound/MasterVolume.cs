using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if !UNITY_EDITOR
using FMOD.Studio;
#endif
using System.IO;

public class MasterVolume : MonoBehaviour
{
    public static MasterVolume volumeHandler;
    public GameSettings gameSettings;

    void Awake()
    {
        volumeHandler = this;
    }

    /*

#if !UNITY_EDITOR
    FMOD.Studio.Bus masterBus;


    void Start()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    void Update()
    {
        masterBus.setFaderLevel(volume);
    }

#else

    */

    GameObject[] bgMusic;
    GameObject[] soundFX;

    void Start()
    {
        UpdateAllLevels();
    }

    public void UpdateAllLevels()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        bgMusic = GameObject.FindGameObjectsWithTag("BG");
        soundFX = GameObject.FindGameObjectsWithTag("SFX");

        foreach (GameObject clip in bgMusic)
        {
            clip.GetComponent<AudioSource>().volume = gameSettings.bgVolume * gameSettings.masterVolume;
        }

        foreach (GameObject clip in soundFX)
        {
            clip.GetComponent<AudioSource>().volume = gameSettings.sfxVolume * gameSettings.masterVolume;
        }
    }


//#endif
}