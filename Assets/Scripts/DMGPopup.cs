using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DMGPopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro textMeshPro;
    [SerializeField]
    private float lifetime = 5f;
    private void Awake() {
        textMeshPro = transform.GetComponent<TextMeshPro>();
    }
    public void setText(string text, int mode) {
        textMeshPro.SetText(text);
        this.GetComponent<Animator>().SetInteger("Mode", mode);
        StartCoroutine(kill());
    }

    private IEnumerator kill() {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
}
