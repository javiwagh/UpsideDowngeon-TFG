using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private Animator blackFadeAnimator;
    [SerializeField]
    private Animator menuAnimator;
    
    public void StartGame() {
        StartCoroutine(LoadGame(1));
    }
    public void StartTutorial() {
        StartCoroutine(LoadGame(0));
    }
    public void Quit() {
        StartCoroutine(QuitGame());
    }

    public void Options() {
        menuAnimator.SetTrigger("Options");
    }

    public void Back() {
        menuAnimator.SetTrigger("Back");
    }

    private IEnumerator LoadGame(int level) {
        blackFadeAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(2.5f);
        switch (level) {
            case 1:
                SceneManager.LoadScene("Level_1");
                break;
            default:
                SceneManager.LoadScene("Tutorial");
                break;
        }
    }

    private IEnumerator QuitGame() {
        blackFadeAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
    }
}
