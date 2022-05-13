using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpawnButtonManager : MonoBehaviour
{
    [SerializeField]
    private GameObject spawnPannel;

    [SerializeField]
    private Button spawnTrollButton;
    private TextMeshProUGUI trollText;
    [SerializeField]
    private Button spawnGoblinButton;
    private TextMeshProUGUI goblinText;
    [SerializeField]
    private Button spawnRatButton;
    private TextMeshProUGUI ratText;
    [SerializeField]
    private Button spawnSpiderButton;
    private TextMeshProUGUI spiderText;

    private Color originalTextColor;
    private Color disabledColor = Color.red;

    private void Awake() {
        trollText = spawnTrollButton.GetComponentInChildren<TextMeshProUGUI>();
        goblinText = spawnGoblinButton.GetComponentInChildren<TextMeshProUGUI>();
        ratText = spawnRatButton.GetComponentInChildren<TextMeshProUGUI>();
        spiderText = spawnSpiderButton.GetComponentInChildren<TextMeshProUGUI>();
        originalTextColor = trollText.color;
    }

    /*private void OnMouseEnter() {
        toggleSpawnPannel(true);
    }

    private void OnMouseExit() {
        toggleSpawnPannel(false);
    }*/

    public void toggleSpawnPannel(bool show) {
        Animator animator = spawnPannel.GetComponent<Animator>();
        bool state = animator.GetBool("Show");

        animator.SetBool("Show", show);
        spawnTrollButton.interactable = show;
        spawnGoblinButton.interactable = show;
        spawnRatButton.interactable = show;
        spawnSpiderButton.interactable = show;
    }

    public void disableSpawnButtons(int currentMana) {
        if (currentMana < 2) {
            //DISABLE EVERY BUTTON
            spawnTrollButton.interactable = false;
            trollText.color = disabledColor;

            spawnGoblinButton.interactable = false;
            goblinText.color = disabledColor;

            spawnRatButton.interactable = false;
            ratText.color = disabledColor;

            spawnSpiderButton.interactable = false;
            spiderText.color = disabledColor;
        }
        else if (currentMana < 3) {
            //DISABLE EVERY BUTTON
            spawnTrollButton.interactable = false;
            trollText.color = disabledColor;

            spawnGoblinButton.interactable = false;
            goblinText.color = disabledColor;

            spawnRatButton.interactable = false;
            ratText.color = disabledColor;

            //BUT SPIDER
            spawnSpiderButton.interactable = true;
            spiderText.color = originalTextColor;
        }
        else if (currentMana < 4) {
            //DISABLE EVERY BUTTON
            spawnTrollButton.interactable = false;
            trollText.color = disabledColor;

            spawnGoblinButton.interactable = false;
            goblinText.color = disabledColor;

            //BUT SPIDER
            spawnRatButton.interactable = true;
            ratText.color = originalTextColor;

            spawnSpiderButton.interactable = true;
            spiderText.color = originalTextColor;
        }
        else if (currentMana < 5) {
            //DISABLE TROLL BUTTON
            spawnTrollButton.interactable = false;
            trollText.color = disabledColor;
            
            //ENABLE EVERY OTHER BUTTON
            spawnGoblinButton.interactable = true;
            goblinText.color = originalTextColor;

            spawnRatButton.interactable = true;
            ratText.color = originalTextColor;

            spawnSpiderButton.interactable = true;
            spiderText.color = originalTextColor;
        }
        else {
            //ENABLE EVERY BUTTON
            spawnTrollButton.interactable = true;
            trollText.color = originalTextColor;

            spawnGoblinButton.interactable = true;
            goblinText.color = originalTextColor;

            spawnRatButton.interactable = true;
            ratText.color = originalTextColor;

            spawnSpiderButton.interactable = true;
            spiderText.color = originalTextColor;
        }
        
    }
}
