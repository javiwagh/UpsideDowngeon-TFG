using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public Player player;
    public AudioSource monsterTheme;
    public AudioSource adventurerTheme;
    public AudioSource endTurnSFX;
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

    public Tutorial basicTutorial;

    public bool monstersTurn {get; private set;} = true;
    public bool endedStage {get; private set;} = false;
    private void Start() {
        monstersTurn = true;
        hexGrid = FindObjectOfType<HexGrid>();
        unitManager = FindObjectOfType<UnitManager>();

        StartCoroutine(GameStartCoroutine());
        Wait();
    }

    void Update() {
        if (gameState == GameState.MonstersWin) {
            endStage(true);
        }
        else if (gameState == GameState.AdventurersWin) {
            endStage(false);
        }
    }

    public void endTurn() {
        monstersTurn = !monstersTurn;
        if(monstersTurn) {
            Debug.Log("It's monster's turn!");
            endTurnSFX.Play();
            adventurerTheme.Stop();
            monsterTheme.Play();

            endTurnButton.interactable = true;
            player.manaPoints += 5;
            if (player.manaPoints > 10) player.manaPoints = 10;
            player.updateMana();
            spawnPannel.toggleSpawnPannel(true);
        }
        else {
            Debug.Log("It's adventurer's turn!");
            endTurnSFX.Play();
            monsterTheme.Stop();
            adventurerTheme.Play();

            endTurnButton.interactable = false;
            //player.manaPoints = 0;
            //player.updateMana();
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
        if (monstersWin) blackFadeAnimator.SetTrigger("MonsterWin");
        else blackFadeAnimator.SetTrigger("AdventurerWin");
    }

    public bool isStageEnded() {
        return endedStage;
    }

    private IEnumerator GameStartCoroutine() {
        hexGrid.UpdateTiles();
        yield return new WaitForSeconds(0.1f);
        unitManager.updateUnits();
        yield return new WaitForSeconds(0.1f);
        if (basicTutorial != null) {
            basicTutorial.StartTutorial();
            unitManager.runningBasicTutorial = true;
        }        
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
