using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using System.Linq;


public class SettingsMenu : MonoBehaviour
{

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;
    public Toggle fullscreenToggle;
    public Slider volumeSlider;
    public AudioMixer audioMixer;

    Resolution[] resolutions;

    // Start is called before the first frame update
    void Start()
    {
        //Gather valid fullscreen resolutions
        resolutions = Screen.resolutions.Select(Resolution => new Resolution { width = Resolution.width, height = Resolution.height }).Distinct().ToArray();
        resolutionDropdown.ClearOptions();
        List<string> resolutionStrings = new List<string>();
        int currentResolutionIndex = 0;
        for (int i=0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if(!resolutionStrings.Contains(option))
                resolutionStrings.Add(option);
            if(resolutions[i].width == Screen.width && 
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        //update resolution dropdown with valid values
        resolutionDropdown.AddOptions(resolutionStrings);

        //initialize all settings values
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = Screen.fullScreen;

        graphicsDropdown.value = QualitySettings.GetQualityLevel();
        graphicsDropdown.RefreshShownValue();

        float value;
        audioMixer.GetFloat("masterVolume", out value);
        volumeSlider.value = Mathf.Pow(10, (value/20));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20);
    }

    public void UpdateResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetGraphicsQuality(int graphicsIndex)
    {
        QualitySettings.SetQualityLevel(graphicsIndex);
    }
}
