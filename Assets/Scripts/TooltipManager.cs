using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _intance;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sideText;
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI actionsText;
    private void Awake() {
        Hide();
        if (_intance != null && _intance != this) 
            Destroy(this.gameObject);
        else _intance = this;
    }

    private void Start() {
        Cursor.visible = true;
        gameObject.SetActive(false);
    }

    private void Update() {
        transform.position = Input.mousePosition;
    }

    public void SetAndShow(string name, string side, List<string> stats, int actions) {
        gameObject.SetActive(true);
        nameText.text = name;
        sideText.text = side;
        actionsText.text = actions.ToString();
        foreach(string stat in stats) {
            statsText.text += stat;
            statsText.text += "\n";
        }
    }

    public void Set(string name, string side, List<string> stats, int actions) {
        //gameObject.SetActive(true);
        nameText.text = name;
        sideText.text = side;
        actionsText.text = actions.ToString();
        foreach(string stat in stats) {
            statsText.text += stat;
            statsText.text += "\n";
        }
    }

    public void Hide() {
        gameObject.SetActive(false);
        nameText.text = string.Empty;
        sideText.text = string.Empty;
        statsText.text = string.Empty;
    }
}
