using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public Player player;
    [SerializeField]
    private HexGrid hexGrid;
    [SerializeField]
     private UnitManager unitManager;
    [SerializeField]
    private HexagonTile endTile;
    [SerializeField]
    private GameObject keyTilePrefab;

    [SerializeField]
    private Animator blackFadeAnimator;
    public bool monstersTurn {get; private set;} = true;
    public bool endedStage {get; private set;} = false;
    private void Start() {
        gameState = GameState.KeyOnBoard;
        monstersTurn = true;
        hexGrid = FindObjectOfType<HexGrid>();
        unitManager = FindObjectOfType<UnitManager>();

        StartCoroutine(GameStartCoroutine());
    }
    public void endTurn() {
        monstersTurn = !monstersTurn;
        if(monstersTurn) {
            Debug.Log("It's monster's turn!");
            player.manaPoints = 10;
            player.updateMana();
        }
        else {
            Debug.Log("It's adventurer's turn!");
            player.manaPoints = 6;
            player.updateMana();
        }
    }

    public void KeyPicked() {
        gameState = GameState.KeyPicked;
        hexGrid.UpdateTiles();
    }

    public void keyDropped(HexagonTile tile) {
        gameState = GameState.KeyOnBoard;
        GameObject key = Instantiate(keyTilePrefab, tile.transform.position, tile.transform.rotation);
        key.transform.SetParent(hexGrid.transform, false);
        key.GetComponent<Key>().setTile(tile.gameObject);
        hexGrid.UpdateTiles();
    }

    public void canEndStage() {
        endTile.enableEnd();
    }

    public void disableEndStage() {
        endTile.resetTileType();
    }

    public void AdventurersWin() {
        gameState = GameState.AdventurersWin;
        endStage(false);
        Debug.Log("*ominous* Adventurers have won ò^ó");
    }

    public void MonstersWin() {
        gameState = GameState.MonstersWin;
        endStage(true);
        Debug.Log("*ominous* Monsters have won O·O");
    }

    public void endStage(bool monstersWin) {
        Debug.Log("ENDING STAGE");
        endedStage = true;
        blackFadeAnimator.SetTrigger("EndStage");
        if (monstersWin) blackFadeAnimator.SetTrigger("MonsterWin");
        else blackFadeAnimator.SetTrigger("AdventurerWin");
    }

    public bool isStageEnded() {
        return endedStage;
    }

    private IEnumerator GameStartCoroutine() {
        hexGrid.UpdateTiles();
        unitManager.updateUnits();
        yield return null;
    } 

    public enum GameState {
        KeyOnBoard,
        KeyPicked,
        MonstersWin,
        AdventurersWin
    }
}
