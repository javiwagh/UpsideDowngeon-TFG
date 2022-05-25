using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Parameters parameters;
    public Button StartGameButton;
    void Start() {
        parameters = FindObjectOfType<Parameters>();

        if (parameters.playedTutorial) StartGameButton.interactable = true;
        else StartGameButton.interactable = false;
    }
}
