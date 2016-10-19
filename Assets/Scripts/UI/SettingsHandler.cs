using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using FMODUnity;

public class SettingsHandler : MonoBehaviour {

    [SerializeField]
    Slider masterVolume;
    [SerializeField]
    Slider sfxVolume;
    [SerializeField]
    Slider bgVolume;

    [SerializeField]
    Toggle fullscreenToggle;

    [SerializeField]
    Dropdown resolutionDropdown;
    [SerializeField]
    Dropdown textureQualityDropdown;

    Resolution[] resolutions;
    GameSettings gameSettings;

    void OnEnable()
    {
        gameSettings = new GameSettings();

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        masterVolume.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        sfxVolume.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        bgVolume.onValueChanged.AddListener(delegate { OnBGVolumeChange(); });

        resolutions = Screen.resolutions;
        foreach(Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }
    }

    public void OnFullscreenToggle()
    {
        gameSettings.fullscreen = Screen.fullScreen = fullscreenToggle.isOn;
    }

    public void OnResolutionChange()
    {
        Screen.SetResolution(resolutions[resolutionDropdown.value].width, resolutions[resolutionDropdown.value].height, Screen.fullScreen);
    }

    public void OnMasterVolumeChange()
    {

    }

    public void OnSFXVolumeChange()
    {

    }

    public void OnBGVolumeChange()
    {

    }
}
