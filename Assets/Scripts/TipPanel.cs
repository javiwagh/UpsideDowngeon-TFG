using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipPanel : MonoBehaviour
{
    public List<string> Tips;
    public TextMeshProUGUI tipBox;
    public int tipIndex;
    private bool show = true;
    private bool changeState = true;

    public Animator animator;
    public Animator tipBoxAnimator;

    public float time = 5f;

    private void Update() {
        time -= Time.deltaTime;
        if (time <= 0f) {
            nextTip();
        }
    }

    public void ShowTips() {
        tipIndex = 0;
        tipBox.text = Tips[tipIndex];
        time = 0.2f * Tips[tipIndex].Length;
    }

    public void Toggle() {
        show = !show;
        animator.SetBool("Show", show);
    }

    public void nextTip() {
        if (show) {
            if (tipIndex == Tips.Count - 1) tipIndex = 0;
            else ++tipIndex;
            time = 0.2f * Tips[tipIndex].Length;
            StartCoroutine(changeTipCoroutine(tipIndex));
        }
    }

    public void prevTip() {
        if (show) {
            if (tipIndex == 0) tipIndex = Tips.Count - 1;
            else --tipIndex;
            time = 0.2f * Tips[tipIndex].Length;
            StartCoroutine(changeTipCoroutine(tipIndex));
        }
    }

    private IEnumerator changeTipCoroutine(int index) {
        changeState = !changeState;
        tipBoxAnimator.SetBool("Change", changeState);
        yield return new WaitForSeconds(0.2f);
        tipBox.text = Tips[tipIndex];
    }
}
