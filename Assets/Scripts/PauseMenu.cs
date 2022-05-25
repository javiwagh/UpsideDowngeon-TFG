using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private Animator blackFadeAnimator;

    [SerializeField]
    private Image gear;

    [SerializeField]
    private List<GameObject> UIElements = new List<GameObject>();
    public GameObject menu;
    private Button pauseButton;
    public CameraController playerCameraController;
    public TooltipManager tooltipManager;
    void Awake()
    {
        pauseButton = this.GetComponent<Button>();
        menu.SetActive(false);
    }

    public void launchPauseMenu() {
        StartCoroutine(pauseCorroutine());
    }

    private IEnumerator pauseCorroutine() {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject element in UIElements) element.SetActive(false);        
        menu.SetActive(true);

        pauseButton.interactable = false;
        gear.enabled = false;

        playerCameraController.playerPanControl = false;
        tooltipManager.paused = true;
    }

    public void closePauseMenu() {
        foreach (GameObject element in UIElements) element.SetActive(true);
        menu.SetActive(false);

        pauseButton.interactable = true;
        gear.enabled = true;

        playerCameraController.playerPanControl = true;
        tooltipManager.paused = false;
    }

    public void RestartGame() {
        StartCoroutine(ReloadGame());
    }
    public void MainMenu() {
        StartCoroutine(LoadMenu());
    }

    public void Quit() {
        StartCoroutine(QuitGame());
    }

    private IEnumerator ReloadGame() {
        blackFadeAnimator.SetTrigger("EndStage");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadMenu() {
        blackFadeAnimator.SetTrigger("EndStage");
        yield return new WaitForSeconds(1.5f);
        Destroy(FindObjectOfType<Parameters>().gameObject);
        SceneManager.LoadScene("Menu");
    }

    private IEnumerator QuitGame() {
        blackFadeAnimator.SetTrigger("EndStage");
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
    }
}
