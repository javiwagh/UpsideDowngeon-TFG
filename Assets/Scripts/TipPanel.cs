using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TipPanel : MonoBehaviour
{
    public List<string> Tips;
    public TextMeshProUGUI tipBox;
    public int tipIndex;

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

    public void nextTip() {
        if (tipIndex == Tips.Count - 1) tipIndex = 0;
        else ++tipIndex;
        time = 0.2f * Tips[tipIndex].Length;
        tipBox.text = Tips[tipIndex];
    }

    public void prevTip() {
        if (tipIndex == 0) tipIndex = Tips.Count - 1;
        else --tipIndex;
        time = 0.2f * Tips[tipIndex].Length;
        tipBox.text = Tips[tipIndex];
    }
}
