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
    public SelectionManager selectionManager;
    public CameraController playerCameraController;
    public Animator manaAnimator;
    private int tutorialLevel = 0;
    private Transform lastTarget;

    public Button continueButton;
    public Button skipButton;
    public TipPanel tipPanel;

    public GameObject pauseButton;
    public bool basicTutorial;
    public bool running;

    void Awake() {
        running = false;
        foreach (GameObject panel in tutorialPanels) panel.SetActive(false);
        skipButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
    }

    public void StartTutorial() {
        unitManager.runningTutorial = true;
        running = true;
        skipButton.gameObject.SetActive(true);
        continueButton.gameObject.SetActive(true);
        
        continueButton.interactable = false;
        skipButton.interactable = false;
        foreach (GameObject UIElement in UIElements) {
            UIElementsToHide.Add(UIElement.name, UIElement);
            Debug.Log(UIElement.name);
        }
        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial() {
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
        yield return new WaitForSeconds(1.5f);
        continueButton.interactable = true;
    }

    public void ContinueTutorial() {
        if (!running) return;
        if (tutorialLevel < tutorialPanels.Count - 1) {
            if (!UIElementsToHide["End Turn"].gameObject.activeInHierarchy) skipButton.gameObject.SetActive(true);
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
        running = false;
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
        playerCameraController.freeCamera();

        selectionManager.allowSelection = true;
        //selectionManager.allowUnitSelection = true;
        //selectionManager.allowTileSelection = true;

        if (basicTutorial) unitManager.runningBasicTutorial = false;
        else {
            unitManager.runningMovementTutorial = false;
            unitManager.runningTutorial = false;
        }
        unitManager.ClearSelection();
        this.gameObject.SetActive(false);
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
            case "AllowSelection":
                AllowSelection();
            break;
            case "LockSelection":
                LockSelection();
            break;
            case "ListenTileSelection":
                ListenTileSelection();
            break;
            case "ListenUnitMovement":
                ListenUnitMovement();
            break;
        }
        tutorialPanels[tutorialLevel].SetActive(true);
    }

    private void AllowSpawn() {
        if (!running) return;
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        UIElementsToHide["Mana panel"].SetActive(true);
        manaAnimator.SetBool("Show", true);
    }

    private void AllowSelection() {
        if (!running) return;
        continueButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(false);
        selectionManager.allowSelection = true;
    }

    private void LockSelection() {
        if (!running) return;
        selectionManager.allowSelection = false;
    }

    private void ListenUnitMovement() {
        if (!running) return;
        unitManager.listenUnitMovement = true;
        ListenTileSelection();
    }

    private void ListenTileSelection() {
        if (!running) return;
        playerCameraController.focusOn(lastTarget);
        continueButton.gameObject.SetActive(false);
        selectionManager.allowSelection = true;
        //selectionManager.allowUnitSelection = false;
    }

    private void ShowMana() {
        if (!running) return;     
        UIElementsToHide["Mana panel"].SetActive(true);
    }

    private void ShowEndTurn() {
        if (!running) return;
        skipButton.gameObject.SetActive(false);
        UIElementsToHide["End Turn"].SetActive(true);
    }

    public void ClickedSpawn() {
        if (!running) return;
        manaAnimator.SetBool("Show", false);
        ContinueTutorial();
    }

    public void PlayerActionDone(Transform target) {
        if (!running) return;
        lastTarget = target;
        playerCameraController.focusOn(target);
        continueButton.gameObject.SetActive(true);
        ContinueTutorial();
    }
}
