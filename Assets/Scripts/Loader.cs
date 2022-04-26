using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    [SerializeField]
    private Animator blackFadeAnimator;
    public void StartGame() {
        StartCoroutine(LoadGame(1));
    }
    public void StartTutorial() {
        StartCoroutine(LoadGame(0));
    }
    public void Quit() {
        StartCoroutine(QuitGame());
    }

    private IEnumerator LoadGame(int level) {
        blackFadeAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(1.5f);
        switch (level) {
            case 1:
                SceneManager.LoadScene("Level_1");
                break;
            default:
                SceneManager.LoadScene("tutorial");
                break;
        }
    }

    private IEnumerator QuitGame() {
        blackFadeAnimator.SetTrigger("Out");
        yield return new WaitForSeconds(1.5f);
        Application.Quit();
    }
}
