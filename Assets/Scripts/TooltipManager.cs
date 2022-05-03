using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _intance;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI sideText;
    //public TextMeshProUGUI statsText;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private GameObject StatsPlusSkillPanel;
    [SerializeField]
    private GameObject poisonousIcon;
    [SerializeField]
    private GameObject paralyseIcon;
    [SerializeField]
    private GameObject stabIcon;

    [SerializeField]
    private GameObject paralysedStateIcon;
    [SerializeField]
    private GameObject poisonedStateIcon;
    [SerializeField]
    private GameObject poisonedPlusParalysedStateIcon;

    [SerializeField]
    private TextMeshProUGUI HP;
    [SerializeField]
    private TextMeshProUGUI SPEED;
    [SerializeField]
    private TextMeshProUGUI ATK;

    public TextMeshProUGUI actionsText;
    public bool paused;
    private void Awake() {
        Hide();
        if (_intance != null && _intance != this) 
            Destroy(this.gameObject);
        else _intance = this;
    }

    private void Start() {
        Cursor.visible = true;
        gameObject.SetActive(false);
        animator.SetBool("Show", false);
    }

    private void Update() {
        transform.position = Input.mousePosition;
    }

    public void SetAndShow(string name, string side, Dictionary<string, string> stats, int actions) {
        Set(name, side, stats, actions);
        /*nameText.text = name;
        sideText.text = side;
        actionsText.text = actions.ToString();
        HP.text = stats["HP"];
        SPEED.text = stats["SPEED"];
        ATK.text = stats["ATK"];
        foreach(string key in stats.Keys) {
            string stat = stats[key];
            statsText.text += stat;
            statsText.text += "\n";
        }*/
        if (!paused) {
            gameObject.SetActive(true);
            animator.SetBool("Show", true);
        }        
    }

    public void Set(string name, string side, Dictionary<string, string> stats, int actions) {
        //gameObject.SetActive(true);
        nameText.text = name;
        sideText.text = side;
        actionsText.text = actions.ToString();

        poisonedPlusParalysedStateIcon.SetActive(false);
        poisonedStateIcon.SetActive(false);
        paralysedStateIcon.SetActive(false);
        
        string value = "";
        if (stats.TryGetValue("POISON", out value)) {
            if (stats.TryGetValue("PARALYSE", out value)) 
                poisonedPlusParalysedStateIcon.SetActive(true);
            else poisonedStateIcon.SetActive(true);
        }
        else if (stats.TryGetValue("PARALYSE", out value)) paralysedStateIcon.SetActive(true);

        HP.text = stats["HP"];
        SPEED.text = stats["SPEED"];
        ATK.text = stats["ATK"];

        poisonousIcon.SetActive(false);
        paralyseIcon.SetActive(false);
        stabIcon.SetActive(false);
        
        if (stats.TryGetValue("SKILL", out value))
            switch (value) {
                case "Poisonous":
                    poisonousIcon.SetActive(true);
                    return;
                case "Paralyse":
                    paralyseIcon.SetActive(true);
                    return;
                case "Stab":
                    stabIcon.SetActive(true);
                    return;
            }
    }

    public void Hide() {
        animator.SetBool("Show", false);
        gameObject.SetActive(false);
        nameText.text = string.Empty;
        sideText.text = string.Empty;
        //statsText.text = string.Empty;
        HP.text = string.Empty;
        SPEED.text = string.Empty;
        ATK.text = string.Empty;
    }
}
