using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> tutorialPanels = new List<GameObject>();
    public List<GameObject> UIElements = new List<GameObject>();
    private Dictionary<string, GameObject> UIElementsToHide = new Dictionary<string, GameObject>();
    public GameManager gameManager;
    public UnitManager unitManager;
    public CameraController playerCameraController;
    public Animator manaAnimator;
    private int tutorialLevel = 0;

    public Button continueButton;
    public Button skipButton;
    public TipPanel tipPanel;

    public GameObject pauseButton;
    
    void Start()
    {
        continueButton.interactable = false;
        skipButton.interactable = false;
        foreach (GameObject UIElement in UIElements) {
            UIElementsToHide.Add(UIElement.name, UIElement);
            Debug.Log(UIElement.name);
        } 
        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial() {
        foreach (GameObject panel in tutorialPanels) panel.SetActive(false);
        foreach (string key in UIElementsToHide.Keys) UIElementsToHide[key].SetActive(false);
        playerCameraController.playerPanControl = false;

        tutorialLevel = 0;        
        ShowTutorialPanel();
        yield return new WaitForSeconds(1.5f);
        continueButton.interactable = true;
        skipButton.interactable = true;
    }

    private IEnumerator lockClick() {
        continueButton.interactable = false;
        yield return new WaitForSeconds(1f);
        continueButton.interactable = true;
    }

    public void ContinueTutorial() {
        if (tutorialLevel < tutorialPanels.Count - 1) {
            HideTutorialPanel();
            ++tutorialLevel;
            ShowTutorialPanel();
            StartCoroutine(lockClick());
        }
        else {
            SkipTutorial();
        }
    }

    public void SkipTutorial() {
        pauseButton.SetActive(true);
        continueButton.interactable = false;
        skipButton.interactable = false;
        playerCameraController.playerPanControl = true;
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<CanvasGroup>().blocksRaycasts = false;

        foreach (string key in UIElementsToHide.Keys) UIElementsToHide[key].SetActive(true);
        manaAnimator.SetBool("Show", true);
        tipPanel.gameObject.SetActive(true);
        tipPanel.ShowTips();
        gameObject.SetActive(false);
        playerCameraController.freeCamera();
        HideTutorialPanel();
    }

    public void HideTutorialPanel() {
        tutorialPanels[tutorialLevel].SetActive(false);
    }

    public void ShowTutorialPanel() {
        if (tutorialPanels[tutorialLevel].GetComponent<TutorialPhase>().focus != null) 
            playerCameraController.focusOn(tutorialPanels[tutorialLevel].GetComponent<TutorialPhase>().focus);
        switch(tutorialPanels[tutorialLevel].GetComponent<TutorialPhase>().methodName) {
            case "AllowSpawn":
                AllowSpawn();
            break;
            case "ShowMana":
                ShowMana();
            break;
            case "ShowEndTurn":
                ShowEndTurn();
            break;
        }
        tutorialPanels[tutorialLevel].SetActive(true);
    }

    private void AllowSpawn() {
        continueButton.gameObject.SetActive(false);
        UIElementsToHide["Mana panel"].SetActive(true);
        manaAnimator.SetBool("Show", true);
    }

    private void ShowMana() {        
        UIElementsToHide["Mana panel"].SetActive(true);
    }

    private void ShowEndTurn() {
        skipButton.gameObject.SetActive(false);
        UIElementsToHide["End Turn"].SetActive(true);
    }

    public void ClickedSpawn() {
        manaAnimator.SetBool("Show", false);
        //UIElementsToHide["Mana panel"].SetActive(false);
        ContinueTutorial();
    }

    public void Spawned(Transform monsterInstance) {
        playerCameraController.focusOn(monsterInstance);
        continueButton.gameObject.SetActive(true);
        ContinueTutorial();
    }
}
