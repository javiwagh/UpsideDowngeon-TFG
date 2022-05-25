using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Languages : MonoBehaviour
{
    public bool lastEspanolValue;
    public string stringInSpanish;
    public string stringInEnglish;
    public TextMeshProUGUI Text;
    public Parameters parameters;
    void Awake() {
        Text = gameObject.GetComponent<TextMeshProUGUI>();
        parameters = FindObjectOfType<Parameters>();
    }

    void OnEnable() {
        changeLanguage(parameters.espanol);
    }
    public void changeLanguage(bool espanol)
    {
        if (espanol != lastEspanolValue) {
            lastEspanolValue = espanol;
            if (espanol) Text.text = stringInSpanish.Replace(".", "\n");
            else Text.text = stringInEnglish.Replace(".", "\n");
        }
    }
}
