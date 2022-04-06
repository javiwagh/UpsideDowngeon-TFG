using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private string side;
    List<string> stats = new List<string>();
    public void setInfo(Character character) {
        if (character.unitType != UnitType.Nest) {
            unitName = character.characterName.ToString();
            side = character.unitType.ToString();
            stats.Add("Current Health: " + character.healthPoints);
            stats.Add("Speed: " + character.speed);
            stats.Add("Strength: " + character.meleeDamage);
        }
        else {
            unitName = character.unitType.ToString();
            side = character.nestType.ToString();
        }        
    }

    public void updateHealth(int HP) {
        stats[0] = "Current Health: " + HP;
        TooltipManager._intance.Hide();
        TooltipManager._intance.SetAndShow(unitName, side, stats);
    }
    private void OnMouseEnter() {
        TooltipManager._intance.Hide();
        TooltipManager._intance.SetAndShow(unitName, side, stats);
    }
    private void OnMouseExit() {
        TooltipManager._intance.Hide();
    }
}
