using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameState gameState;
    public bool monstersTurn {get; private set;} = true;
    private void Awake() {
        gameState = GameState.KeyOnBoard;
        monstersTurn = true;
    }
    public void endTurn() {
        monstersTurn = !monstersTurn;
        if(monstersTurn) Debug.Log("It's monster's turn!");
        else Debug.Log("It's adventurer's turn!");
    }
    public enum GameState {
        KeyOnBoard,
        KeyPicked,
        MonstersWin,
        AdventurersWin
    }
}
