using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTip : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private string side;
    private int actions;
    Dictionary<string, string> stats = new Dictionary<string, string>();
    public void setInfo(Character character) {
        if (character.unitType != UnitType.Nest) {
            unitName = character.characterName.ToString();
            side = character.unitType.ToString();
            stats.Add("HP", "Current Health: " + character.healthPoints);
            stats.Add("SPEED", "Speed: " + character.speed);
            stats.Add("STRENGTH", "Strength: " + character.meleeDamage);
            if (character.skill != Skill.None) stats.Add("SKILL", "Skill: " + character.skill);
            if (this.GetComponent<Unit>().poisonCounter > 0) stats.Add("POISON", "Poisoned");
            if (this.GetComponent<Unit>().paralysed) stats.Add("PARALYSE", "Paralysed");
            actions = character.GetComponent<Unit>().actionPoints;
        }
        else {
            unitName = character.unitType.ToString();
            side = character.nestType.ToString();
        }        
    }

    public void updateHealth(int HP) {
        stats["HP"] = "Current Health: " + HP;

        string value = "";
        if (this.GetComponent<Unit>().poisonCounter > 0) {
            if (!stats.TryGetValue("POISON", out value)) stats.Add("POISON", "Poisoned");
        }
        else {
            if (stats.TryGetValue("POISON", out value)) stats.Remove("POISON");
        }

        if (this.GetComponent<Unit>().paralysed) {
            if (!stats.TryGetValue("PARALYSE", out value)) stats.Add("PARALYSE", "Paralysed");
        }
        else {
            if (stats.TryGetValue("PARALYSE", out value)) stats.Remove("PARALYSE");
        }

        
        TooltipManager._intance.Hide();
        TooltipManager._intance.Set(unitName, side, stats, actions);
    }
    public void updateActionPoints(int actionPoints) {
        actions = actionPoints;
        TooltipManager._intance.Hide();
        TooltipManager._intance.Set(unitName, side, stats, actions);
    }
    private void OnMouseEnter() {
        TooltipManager._intance.Hide();
        TooltipManager._intance.SetAndShow(unitName, side, stats, actions);
    }
    private void OnMouseExit() {
        TooltipManager._intance.Hide();
    }
}
