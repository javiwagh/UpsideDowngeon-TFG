using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeManager : MonoBehaviour
{
    public Parameters parameters;
    void Awake() {
        parameters = FindObjectOfType<Parameters>();
        SetVolume(parameters.generalVolume);
    }

    void OnEnable() {
        SetVolume(parameters.generalVolume);
    }

    public void SetVolume(float newVolume) {
        AudioListener.volume = newVolume;
    }
}
