using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    [SerializeField]
    private HexGrid hexGrid;
    [SerializeField]
    private HexagonTile endTile;
    [SerializeField]
    private GameObject keyTilePrefab;
    public bool monstersTurn {get; private set;} = true;
    private void Awake() {
        gameState = GameState.KeyOnBoard;
        monstersTurn = true;
        hexGrid = FindObjectOfType<HexGrid>();
    }
    public void endTurn() {
        monstersTurn = !monstersTurn;
        if(monstersTurn) Debug.Log("It's monster's turn!");
        else Debug.Log("It's adventurer's turn!");
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
        Debug.Log("*ominous* Adventurers have won ò^ó");
    }

    public enum GameState {
        KeyOnBoard,
        KeyPicked,
        MonstersWin,
        AdventurersWin
    }
}
