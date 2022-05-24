using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameters : MonoBehaviour
{
    public bool espanol = false;

    void Awake() {
        DontDestroyOnLoad(this.gameObject);
    }

    public void ToggleLanguage() {
        espanol = !espanol;
        foreach (Languages item in FindObjectsOfType<Languages>()) {
            item.changeLanguage(espanol);
        }
    }
}
