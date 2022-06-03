using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int manaPoints;
    [SerializeField]
    private TextMeshProUGUI manaText;
    [SerializeField]
    private SpawnButtonManager spawnPannel;
    private void Awake() {
        //manaPoints = 10;
        updateMana();
    }

    public void updateMana() {
        manaText.text = manaPoints.ToString();
        spawnPannel.disableSpawnButtons(manaPoints);
    }
}
