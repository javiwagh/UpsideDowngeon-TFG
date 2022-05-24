using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ToolTip : MonoBehaviour
{
    [SerializeField]
    private string unitName;
    [SerializeField]
    private string side;
    private int actions;
    Dictionary<string, string> stats = new Dictionary<string, string>();
    public bool lastEspanolValue;
    public Parameters parameters;
    void SetLanguage() {
        parameters = FindObjectOfType<Parameters>();
        changeLanguage(parameters.espanol);
    }

    public void changeLanguage(bool espanol)
    {
        if (espanol != lastEspanolValue) lastEspanolValue = espanol;
    }

    public void setInfo(Character character) {
        SetLanguage();
        if (character.unitType != UnitType.Nest) {
            if (!lastEspanolValue) unitName = character.characterName.ToString();
            else {
                switch(character.characterName) {
                    case(CharacterName.Goblin):
                        unitName = "Trasgo";
                    break;
                    case(CharacterName.Troll):
                        unitName = "Troll";
                    break;
                    case(CharacterName.Spider):
                        unitName = "Araña";
                    break;
                    case(CharacterName.Rat):
                        unitName = "Rata";
                    break;
                    case(CharacterName.Rogue):
                        unitName = "Pícaro";
                    break;
                    case(CharacterName.Warrior):
                        unitName = "Guerrera";
                    break;
                    case(CharacterName.Bard):
                        unitName = "Bardo";
                    break;
                }
            }
            if (!lastEspanolValue) side = character.unitType.ToString();
            else {
                switch(character.side) {
                    case(Side.Adventurers):
                        side = "Aventureros";
                    break;
                    case(Side.Monsters):
                        side = "Monstruos";
                    break;
                }
            }
            
            stats.Add("HP", character.healthPoints.ToString());
            stats.Add("SPEED", character.speed.ToString());
            stats.Add("ATK", character.meleeDamage.ToString());
            if (character.skill != Skill.None) stats.Add("SKILL", character.skill.ToString());
            if (this.GetComponent<Unit>().poisonCounter > 0) stats.Add("POISON", "Poisoned");
            if (this.GetComponent<Unit>().paralysed) stats.Add("PARALYSE", "Paralysed");
            actions = character.GetComponent<Unit>().actionPoints;
        }   
    }

    public void updateHealth(int HP) {
        stats["HP"] = HP.ToString();

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
