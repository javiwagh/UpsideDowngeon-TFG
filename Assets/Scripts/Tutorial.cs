using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public List<GameObject> tutorialPanels;
    public GameManager gameManager;
    public CameraController playerCameraController;
    private int tutorialLevel = 0;

    public Button continueButton;
    public Button skipButton;
    public TipPanel tipPanel;

    public GameObject pauseButton;
    
    void Start()
    {
        pauseButton.SetActive(false);
        tipPanel.gameObject.SetActive(false);
        continueButton.interactable = false;
        skipButton.interactable = false;
        playerCameraController.playerPanControl = false;
        StartCoroutine(RunTutorial());
    }

    private IEnumerator RunTutorial() {
        foreach (GameObject panel in tutorialPanels) panel.SetActive(false);
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
        HideTutorialPanel();
        tipPanel.gameObject.SetActive(true);
        tipPanel.ShowTips();
        gameObject.SetActive(false);
    }

    public void HideTutorialPanel() {
        tutorialPanels[tutorialLevel].SetActive(false);
    }

    public void ShowTutorialPanel() {
        tutorialPanels[tutorialLevel].SetActive(true);
    }
}
