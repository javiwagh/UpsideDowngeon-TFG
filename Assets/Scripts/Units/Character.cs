using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public ToolTip toolTip;
    [SerializeField]
    public CharacterName characterName;
    [HideInInspector]
    public UnitType unitType;
    public Side side;
    public MonsterType? monsterType;
    public NestType? nestType;
    public AdventurerType? adventurerType;
    public int cost = 0;
    public int healthPoints = 10;
    public int speed = 2;
    public int meleeDamage = 2;

    private void Awake() {
        asignTypes();
        toolTip = this.gameObject.GetComponent<ToolTip>();
        toolTip.setInfo(this);
    }

    private void asignTypes() {
        switch(characterName) {
            case CharacterName.Goblin:
                unitType = UnitType.Monster;
                monsterType = MonsterType.Goblin;
                side = Side.Monsters;
                nestType = null;
                adventurerType = null;
            break;
            case CharacterName.Troll:
                unitType = UnitType.Monster;
                monsterType = MonsterType.Troll;
                side = Side.Monsters;
                nestType = null;
                adventurerType = null;
            break;
            case CharacterName.Spider:
                unitType = UnitType.Monster;
                monsterType = MonsterType.Spider;
                nestType = null;
                adventurerType = null;
            break;
            case CharacterName.Rat:
                unitType = UnitType.Monster;
                side = Side.Monsters;
                monsterType = MonsterType.Rat;
                nestType = null;
                adventurerType = null;
            break;
            case CharacterName.Spiders_nest:
                unitType = UnitType.Nest;
                side = Side.Monsters;
                monsterType = null;
                nestType = NestType.Spiders;
                adventurerType = null;
            break;
            case CharacterName.Rats_nest:
                unitType = UnitType.Nest;
                side = Side.Monsters;
                monsterType = null;
                nestType = NestType.Rats;
                adventurerType = null;
            break;
            case CharacterName.Rogue:
                unitType = UnitType.Adventurer;
                side = Side.Adventurers;
                monsterType = null;
                nestType = null;
                adventurerType = AdventurerType.Rogue;
            break;
            case CharacterName.Warrior:
                unitType = UnitType.Adventurer;
                side = Side.Adventurers;
                monsterType = null;
                nestType = null;
                adventurerType = AdventurerType.Warrior;
            break;
            case CharacterName.Bard:
                unitType = UnitType.Adventurer;
                side = Side.Adventurers;
                monsterType = null;
                nestType = null;
                adventurerType = AdventurerType.Bard;
            break;
        }
        
        if (monsterType != null) {
            Debug.Log($"My name is {characterName}. I am a {unitType}, {monsterType} indeed.");
        }
        else if (nestType != null) {
            Debug.Log($"My name is {characterName}. I am a {unitType}, of {nestType} indeed.");
        }
        else if (adventurerType != null) {
            Debug.Log($"My name is {characterName}. I am a {unitType}, {adventurerType} indeed.");
        }
    }
}

public enum CharacterName {
    Goblin,
    Troll,
    Spider,
    Rat,
    Spiders_nest,
    Rats_nest,
    Rogue,
    Warrior,
    Bard
}

public enum UnitType {
    Monster,
    Nest,
    Adventurer
}

public enum Side {
    Monsters,
    Adventurers
}

public enum MonsterType {
    Goblin,
    Troll,
    Spider, 
    Rat
}

public enum NestType {
    Spiders,
    Rats
}

public enum AdventurerType {
    Rogue,
    Warrior,
    Bard
}