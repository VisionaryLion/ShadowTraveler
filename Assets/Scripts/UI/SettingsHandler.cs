using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if !UNITY_EDITOR
using FMOD.Studio;
#endif
using System.IO;

public class SettingsHandler : MonoBehaviour
{

    [SerializeField]
    Slider masterVolumeSlider;
    [SerializeField]
    Slider sfxSlider;
    [SerializeField]
    Slider bgMusicSlider;

    [SerializeField]
    Toggle fullscreenToggle;

    [SerializeField]
    Dropdown resolutionDropdown;
    [SerializeField]
    Dropdown textureQualityDropdown;

    public Button applyButton;

    Resolution[] resolutions;
    public GameSettings gameSettings;

    void OnEnable()
    {
        gameSettings = new GameSettings();

        //fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        //resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });

        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });

        sfxSlider.onValueChanged.AddListener(delegate { OnSFXVolumeChange(); });
        bgMusicSlider.onValueChanged.AddListener(delegate { OnBGVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });
        
        /*
        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

        */

        LoadSettings();
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
        PlayerPrefs.SetFloat("Master Volume", gameSettings.masterVolume);

        gameSettings.masterVolume = masterVolumeSlider.value;
    }

    public void OnSFXVolumeChange()
    {
        PlayerPrefs.SetFloat("SFX Volume", gameSettings.masterVolume);

        gameSettings.sfxVolume = sfxSlider.value;
    }

    public void OnBGVolumeChange()
    {
        PlayerPrefs.SetFloat("BG Volume", gameSettings.masterVolume);

        gameSettings.bgVolume = bgMusicSlider.value;
    }

    public void OnApplyButtonClick()
    {
        SaveSettings();
        MasterVolume.volumeHandler.UpdateAllLevels();
    }

    public void SaveSettings()
    {
        string jsonData = JsonUtility.ToJson(gameSettings, true);
        File.WriteAllText(Application.persistentDataPath + "/gamesettings.json", jsonData);
    }

    public void LoadSettings()
    {
        gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));

        masterVolumeSlider.value = gameSettings.masterVolume;
        sfxSlider.value = gameSettings.sfxVolume;
        bgMusicSlider.value = gameSettings.bgVolume;

        //resolutionDropdown.value = gameSettings.resolutionIndex;
        //fullscreenToggle.isOn = gameSettings.fullscreen;
    }
}