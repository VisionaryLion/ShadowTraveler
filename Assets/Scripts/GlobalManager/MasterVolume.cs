using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if !UNITY_EDITOR
using FMOD.Studio;
#endif
using System.IO;

public class MasterVolume : MonoBehaviour
{

    public GameSettings gameSettings;

    [Range(0, 100)]
    public float volume = 1.0f;

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

    AudioSource[] bgMusic;
    AudioSource[] soundFX;

    void Start()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        foreach (AudioSource clip in bgMusic)
        {
            clip.volume = gameSettings.bgVolume;
        }

        foreach (AudioSource clip in soundFX)
        {
            clip.volume = gameSettings.sfxVolume;
        }
    }
#endif
}
