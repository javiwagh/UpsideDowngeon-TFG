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
    private List<GameObject> UIElements = new List<GameObject>();
    public GameObject menu;
    private Button pauseButton;
    public CameraController playerCameraController;
    void Awake()
    {
        pauseButton = this.GetComponent<Button>();
        menu.SetActive(false);
    }

    public void launchPauseMenu() {
        foreach (GameObject element in UIElements) element.SetActive(false);        
        menu.SetActive(true);

        pauseButton.interactable = false;
        gameObject.GetComponent<Image>().enabled = false;

        playerCameraController.playerPanControl = false;
    }

    public void closePauseMenu() {
        foreach (GameObject element in UIElements) element.SetActive(true);
        menu.SetActive(false);

        pauseButton.interactable = true;
        gameObject.GetComponent<Image>().enabled = true;

        playerCameraController.playerPanControl = true;
    }

    public void RestartGame() {
        StartCoroutine(ReloadGame());
    }
    public void MainMenu() {
        StartCoroutine(LoadMenu());
    }

    private IEnumerator ReloadGame() {
        blackFadeAnimator.SetTrigger("EndStage");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private IEnumerator LoadMenu() {
        blackFadeAnimator.SetTrigger("EndStage");
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Menu");
    }
}
