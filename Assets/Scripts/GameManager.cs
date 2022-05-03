using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    [SerializeField]
    private Button endTurnButton;
    [SerializeField]
    private SpawnButtonManager spawnPannel;

    public bool monstersTurn {get; private set;} = true;
    public bool endedStage {get; private set;} = false;
    private void Start() {
        monstersTurn = true;
        hexGrid = FindObjectOfType<HexGrid>();
        unitManager = FindObjectOfType<UnitManager>();

        StartCoroutine(GameStartCoroutine());
        Wait();
    }
    public void endTurn() {
        monstersTurn = !monstersTurn;
        if(monstersTurn) {
            Debug.Log("It's monster's turn!");
            endTurnButton.interactable = true;
            player.manaPoints = 5;
            player.updateMana();
            spawnPannel.toggleSpawnPannel(true);
        }
        else {
            Debug.Log("It's adventurer's turn!");
            endTurnButton.interactable = false;
            player.manaPoints = 0;
            player.updateMana();
            spawnPannel.toggleSpawnPannel(false);
        }
    }

    public void KeyPicked() {
        hexGrid.UpdateTiles();
    }

    /*public void keyDropped(HexagonTile tile) {
        gameState = GameState.KeyOnBoard;
        GameObject key = Instantiate(keyTilePrefab, tile.transform.position, tile.transform.rotation);
        key.transform.SetParent(tile.room.transform, false);
        tile.room.hasKeyTile = true;
        tile.room.keyTile = key.GetComponent<HexagonTile>();
        key.GetComponent<Key>().setTile(tile.gameObject);
        hexGrid.UpdateTiles();
    }*/

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

    public void RemoveUnit(GameObject unit) {
        unitManager.RemoveUnit(unit);
    }

    public void Move() {
        this.gameState = GameState.Moving;
    }
    public void Wait() {
        this.gameState = GameState.Waiting;
    }

    public void followAdventurer(GameObject adventurer) {
        player.GetComponent<CameraController>().followAdventurer(adventurer);
    }

    public void freeCamera() {
        player.GetComponent<CameraController>().freeCamera();
    }

    public enum GameState {
        Moving,
        Waiting,
        MonstersWin,
        AdventurersWin
    }
}
