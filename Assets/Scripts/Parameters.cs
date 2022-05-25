using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Parameters : MonoBehaviour
{
    public bool espanol = false;
    public float generalVolume;
    private VolumeManager volumeManager;
    public Slider mySlider = null;
    public bool playedTutorial;

    void Awake() {
        //PlayerPrefs.DeleteAll();
        DontDestroyOnLoad(this.gameObject);

        string language = PlayerPrefs.GetString("Language");
        float volume = PlayerPrefs.GetFloat("Volume");
        string tuto = PlayerPrefs.GetString("PlayedTutorial");

        if (language == "") {
            if (Application.systemLanguage == SystemLanguage.Spanish) espanol = true;
        } 
        else if (language == "Espanol") espanol = true;
        else espanol = false;

        if (volume == 0f) generalVolume = 0.8f;
        else generalVolume = volume;

        if (tuto == "") playedTutorial = false;
        else playedTutorial = true;

        volumeManager = FindObjectOfType<VolumeManager>();

        foreach (Languages item in FindObjectsOfType<Languages>()) {
            item.changeLanguage(espanol);
        }
        volumeManager.SetVolume(generalVolume);
        if (mySlider != null) mySlider.value = generalVolume;
    }

    public void alreadyPlayedTutorial() {
        PlayerPrefs.SetString("PlayedTutorial", "Yes");
        PlayerPrefs.Save();
    }

    public void ToggleLanguage() {
        espanol = !espanol;
        if (espanol) PlayerPrefs.SetString("Language", "Espanol");
        else PlayerPrefs.SetString("Language", "English");
        PlayerPrefs.Save();
        foreach (Languages item in FindObjectsOfType<Languages>()) {
            item.changeLanguage(espanol);
        }
    }

    public void setVolume(Slider slider) {
        if (mySlider != slider) mySlider = slider;
        generalVolume = slider.value;
        PlayerPrefs.SetFloat("Volume", generalVolume);
        PlayerPrefs.Save();
        volumeManager.SetVolume(generalVolume);
    }
}
