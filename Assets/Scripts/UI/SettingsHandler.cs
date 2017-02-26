using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class SettingsHandler : MonoBehaviour
{

    [SerializeField]
    Slider masterVolumeSlider;

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

        fullscreenToggle.onValueChanged.AddListener(delegate { OnFullscreenToggle(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { OnResolutionChange(); });
        masterVolumeSlider.onValueChanged.AddListener(delegate { OnMasterVolumeChange(); });
        applyButton.onClick.AddListener(delegate { OnApplyButtonClick(); });

        resolutions = Screen.resolutions;
        foreach (Resolution resolution in resolutions)
        {
            resolutionDropdown.options.Add(new Dropdown.OptionData(resolution.ToString()));
        }

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

    public void OnApplyButtonClick()
    {
        SaveSettings();
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
        resolutionDropdown.value = gameSettings.resolutionIndex;
        fullscreenToggle.isOn = gameSettings.fullscreen;
    }
}