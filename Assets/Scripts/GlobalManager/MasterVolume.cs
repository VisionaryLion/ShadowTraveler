using UnityEngine;
using System.Collections;

public class MasterVolume : MonoBehaviour {

    [Range(0, 100)]
    public float volume = 1.0f;
    FMOD.Studio.Bus masterBus;

    void Start()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");

    }

    void Update()
    {
        masterBus.setFaderLevel(volume);
    }
}
